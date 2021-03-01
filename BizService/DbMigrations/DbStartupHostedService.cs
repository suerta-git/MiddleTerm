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
            var tasks = new List<Task>
            {
                createTableIfNotExistsAsync(client, "shows", "Id"),
                createTableIfNotExistsAsync(client, "users", "Id")
            };
            await Task.WhenAll(tasks);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task createTableIfNotExistsAsync(IAmazonDynamoDB client, string tableName, string hashKeyName)
        {
            _logger.LogInformation($"Checking table '{tableName}' existence...");
            var tables = await client.ListTablesAsync();
            if (!tables.TableNames.Contains(tableName))
            {
                _logger.LogInformation($"Table '{tableName}' does not exist");
                _logger.LogInformation($"Creating table '{tableName}'...");
                await client.CreateTableAsync(new CreateTableRequest
                    {
                        TableName = tableName,
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 3,
                            WriteCapacityUnits = 1
                        },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = hashKeyName,
                                KeyType = KeyType.HASH
                            }
                        },
                        AttributeDefinitions = new List<AttributeDefinition>
                        {
                            new AttributeDefinition
                            { 
                                AttributeName = hashKeyName,
                                AttributeType=ScalarAttributeType.S
                            }
                        }
                    });

                _logger.LogInformation($"Checking if table '{tableName}' is availabe...");
                bool isTableAvailable = false;
                while (!isTableAvailable) {
                    await Task.Delay(2000);
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
                _logger.LogInformation($"Table '{tableName}' is availabe...");
            }
            else
            {
                _logger.LogInformation($"Table '{tableName}' exists");
            }
        }
    }
}