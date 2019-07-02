using System.ComponentModel.DataAnnotations;

namespace Postcore.Web.Core.WebModels.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is requred.")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
