namespace StudentAdministration.Client.Models.Subjects
{
    public class GetAllViewModel
    {
        public ICollection<GetAllViewModelItem>? Items { get; set; } = new List<GetAllViewModelItem>();
    }

    public class GetAllViewModelItem
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Department { get; set; }

        public ProfessorViewModelItem? Professor { get; set; }

        public string? PartitionKey { get; set; }
    }

    public class ProfessorViewModelItem
    {
        public string? Id { get; set; }

        public string? FullName { get; set; }

        public string? PartitionKey { get; set; }
    }
}
