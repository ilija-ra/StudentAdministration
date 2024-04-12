using Azure;
using Azure.Data.Tables;

namespace StudentAdministration.Subject.Entities
{
    public class GradeEntity : ITableEntity
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? ProfessorFullName { get; set; }

        public string? StudentFullName { get; set; }

        public string? StudentIndex { get; set; }

        public int? Grade { get; set; }

        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag? ETag { get; set; }

        ETag ITableEntity.ETag { get; set; }
    }
}
