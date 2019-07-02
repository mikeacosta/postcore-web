using System.ComponentModel.DataAnnotations;

namespace Postcore.Web.Core.WebModels.AdManagement
{
    public class CreateAdViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
    }
}
