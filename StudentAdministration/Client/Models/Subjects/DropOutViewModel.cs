using System.ComponentModel.DataAnnotations;

namespace Client.Models.Subjects
{
    public class DropOutViewModel
    {
        [Display(Name = "Subject Id")]
        [Required(ErrorMessage = "Subject Id is required")]
        public string? SubjectId { get; set; }

        [Display(Name = "Student Id")]
        [Required(ErrorMessage = "Student Id is required")]
        public string? StudentId { get; set; }
    }
}
