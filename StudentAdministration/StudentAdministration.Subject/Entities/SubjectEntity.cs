using Azure;
using Azure.Data.Tables;

namespace StudentAdministration.Subject.Entities
{
    public class SubjectEntity : ITableEntity
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public string? ProfessorId { get; set; }

        public string? ProfessorFullName { get; set; }

        public string? ProfessorPartitionKey { get; set; }

        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag? ETag { get; set; }

        ETag ITableEntity.ETag { get; set; }
    }
}
