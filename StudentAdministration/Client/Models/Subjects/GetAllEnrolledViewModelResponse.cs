using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Student id")]
        public string? StudentId { get; set; }

        [Display(Name = "Student partition key")]
        public string? StudentPartitionKey { get; set; }

        [Display(Name = "Professor full name")]
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
