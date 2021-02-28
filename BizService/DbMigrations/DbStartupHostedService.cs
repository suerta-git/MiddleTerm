using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BizService.DbMigrations
{
    public class DbStartupHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public DbStartupHostedService(IServiceProvider serviceProvider, ILogger<DbStartupHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = _serviceProvider.GetRequiredService<IAmazonDynamoDB>();
            await createTableIfNotExistsAsync(client);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task createTableIfNotExistsAsync(IAmazonDynamoDB client)
        {
            const string TABLE_NAME = "shows";
            const string HASH_KEY_NAME = "Id";
            _logger.LogInformation($"Checking table '{TABLE_NAME}' existence...");
            var tables = await client.ListTablesAsync();
            if (!tables.TableNames.Contains(TABLE_NAME))
            {
                _logger.LogInformation($"Table '{TABLE_NAME}' does not exist");
                _logger.LogInformation($"Creating table '{TABLE_NAME}'...");
                await client.CreateTableAsync(new CreateTableRequest
                    {
                        TableName = TABLE_NAME,
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 3,
                            WriteCapacityUnits = 1
                        },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = HASH_KEY_NAME,
                                KeyType = KeyType.HASH
                            }
                        },
                        AttributeDefinitions = new List<AttributeDefinition>
                        {
                            new AttributeDefinition
                            { 
                                AttributeName = HASH_KEY_NAME,
                                AttributeType=ScalarAttributeType.S
                            }
                        }
                    });

                _logger.LogInformation($"Checking if table '{TABLE_NAME}' is availabe...");
                bool isTableAvailable = false;
                while (!isTableAvailable) {
                    await Task.Delay(2000);
                    var tableStatus = await client.DescribeTableAsync(TABLE_NAME);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
                _logger.LogInformation($"Table '{TABLE_NAME}' is availabe...");
            }
            else
            {
                _logger.LogInformation($"Table '{TABLE_NAME}' exists");
            }
        }
    }
}