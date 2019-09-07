using Postcore.Web.Core.ApiModels;

namespace Postcore.Web.Core.WebModels.Home
{
    public class SearchViewModel
    {
        public SearchViewModel(Ad ad)
        {
            Id = ad.Id;
            Title = ad.Title;
            Description = ad.Description;
        }

        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
    }
}
