using System.ComponentModel.DataAnnotations;

namespace StudentAdministration.Client.Models.Subjects
{
    public class GetStudentsBySubjectViewModelResponse
    {
        public ICollection<GetStudentsBySubjectViewModelItem>? Items { get; set; } = new List<GetStudentsBySubjectViewModelItem>();
    }

    public class GetStudentsBySubjectViewModelItem
    {
        public string? Id { get; set; }

        [Display(Name = "Subject id")]
        public string? SubjectId { get; set; }

        [Display(Name = "Subject partition key")]
        public string? SubjectPartitionKey { get; set; }

        [Display(Name = "Student id")]
        public string? StudentId { get; set; }

        [Display(Name = "Student index")]
        public string? StudentIndex { get; set; }

        [Display(Name = "Student partition key")]
        public string? StudentPartitionKey { get; set; }

        [Display(Name = "Student full name")]
        public string? StudentFullName { get; set; }

        public int? Grade { get; set; }
    }
}
