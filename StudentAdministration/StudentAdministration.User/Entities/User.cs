using Azure;
using Azure.Data.Tables;

namespace StudentAdministration.User.Entities
{
    public class User : ITableEntity
    {
        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag? ETag { get; set; }

        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Index { get; set; }

        public string? EmailAddress { get; set; }

        public string? Password { get; set; }

        public string? Salt { get; set; }

        ETag ITableEntity.ETag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
