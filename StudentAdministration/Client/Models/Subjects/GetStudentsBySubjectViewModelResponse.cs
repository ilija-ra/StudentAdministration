namespace StudentAdministration.Client.Models.Subjects
{
    public class GetStudentsBySubjectViewModelResponse
    {
        public ICollection<GetStudentsBySubjectViewModelItem>? Items { get; set; } = new List<GetStudentsBySubjectViewModelItem>();
    }

    public class GetStudentsBySubjectViewModelItem
    {
        public string? Id { get; set; }

        public string? SubjectId { get; set; }

        public string? SubjectPartitionKey { get; set; }

        public string? StudentId { get; set; }

        public string? StudentIndex { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? StudentFullName { get; set; }

        public int? Grade { get; set; }
    }
}
