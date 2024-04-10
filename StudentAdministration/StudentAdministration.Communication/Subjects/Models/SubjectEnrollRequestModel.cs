namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectEnrollRequestModel
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? StudentFullName { get; set; }

        public string? ProfessorFullName { get; set; }

        public int? Grade { get; set; }
    }
}
