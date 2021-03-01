using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace BizService.Models
{
    [DynamoDBTable("users")]
    public class User
    {
        [DynamoDBHashKey]
        [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "Invalid phone number format")]
        [Required]
        public string Id { get; set; }
        public List<string> Votes { get; set; } = new List<string>();

        [DynamoDBIgnore]
        public int VotesCount => Votes.Count;
    }
}