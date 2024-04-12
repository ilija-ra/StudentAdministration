namespace StudentAdministration.Client.Models.Subjects
{
    public class GetAllEnrolledViewModelResponse
    {
        public ICollection<GetAllEnrolledViewModelItem>? Items { get; set; } = new List<GetAllEnrolledViewModelItem>();
    }

    public class GetAllEnrolledViewModelItem
    {
        public string? Id { get; set; }

        public SubjectViewModelItem? Subject { get; set; }

        public string? StudentId { get; set; }

        public string? StudentPartitionKey { get; set; }

        public string? ProfessorFullName { get; set; }
    }

    public class SubjectViewModelItem
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public int? Grade { get; set; }

        public string? PartitionKey { get; set; }
    }
}
