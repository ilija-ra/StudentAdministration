namespace StudentAdministration.Communication.Subjects.Models
{
    public class SubjectGetAllResponseModel
    {
        public ICollection<SubjectGetAllItemModel>? Items { get; set; } = new List<SubjectGetAllItemModel>();
    }

    public class SubjectGetAllItemModel
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public ProfessorItemModel? Professor { get; set; }

        public string? PartitionKey { get; set; }
    }

    public class ProfessorItemModel
    {
        public string? Id { get; set; }

        public string? FullName { get; set; }

        public string? PartitionKey { get; set; }
    }
}
