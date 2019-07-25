using System.ComponentModel.DataAnnotations;

namespace Postcore.Web.Core.WebModels.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
