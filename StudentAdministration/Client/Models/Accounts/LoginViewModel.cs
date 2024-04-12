using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentAdministration.Client.Models.Accounts
{
    public class LoginViewModel
    {
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email address is required")]
        public string? EmailAddress { get; set; }

        [PasswordPropertyText]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
