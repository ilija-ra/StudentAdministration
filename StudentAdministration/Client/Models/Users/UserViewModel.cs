using System.ComponentModel.DataAnnotations;

namespace StudentAdministration.Client.Models.Users
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Index is required")]
        public string? Index { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string? EmailAddress { get; set; }

        public string? PartitionKey { get; set; }
    }
}
