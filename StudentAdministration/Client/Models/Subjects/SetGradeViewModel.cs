using System.ComponentModel.DataAnnotations;

namespace Client.Models.Subjects
{
    public class SetGradeViewModel
    {
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

        [Required(ErrorMessage = "Grade is required")]
        [Range(5, 10, ErrorMessage = "Grade must be between 5 and 10")]
        public int? Grade { get; set; }
    }
}
