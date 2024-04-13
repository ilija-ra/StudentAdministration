using System.ComponentModel.DataAnnotations;

namespace Client.Models.Subjects
{
    public class EnrollViewModel
    {
        [Required(ErrorMessage = "Id is required")]
        public string? Id { get; set; }

        [Display(Name = "Subject id")]
        [Required(ErrorMessage = "Subject id is required")]
        public string? SubjectId { get; set; }

        [Display(Name = "Subject partition key")]
        [Required(ErrorMessage = "Subject partition key is required")]
        public string? SubjectPartitionKey { get; set; }

        [Display(Name = "Student id")]
        [Required(ErrorMessage = "Student id is required")]
        public string? StudentId { get; set; }

        [Display(Name = "Student partition key")]
        [Required(ErrorMessage = "Student partition key is required")]
        public string? StudentPartitionKey { get; set; }

        [Display(Name = "Student index")]
        [Required(ErrorMessage = "Student index is required")]
        public string? StudentIndex { get; set; }

        [Display(Name = "Student full name")]
        [Required(ErrorMessage = "Student full name is required")]
        public string? StudentFullName { get; set; }

        [Display(Name = "Professor full name")]
        [Required(ErrorMessage = "Professor full name is required")]
        public string? ProfessorFullName { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public int? Grade { get; set; }
    }
}
