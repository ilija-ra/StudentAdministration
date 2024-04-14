using Azure;
using Azure.Data.Tables;

namespace StudentAdministration.Communication.Users.Entities
{
    public class UserEntity : ITableEntity
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Index { get; set; }

        public string? EmailAddress { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? Salt { get; set; }

        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag? ETag { get; set; }

        ETag ITableEntity.ETag { get; set; }
    }
}
