using System.ComponentModel.DataAnnotations;

namespace Postcore.Web.Core.WebModels.Accounts
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")] public bool RememberMe { get; set; }
    }
}