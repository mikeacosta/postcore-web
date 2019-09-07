using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Postcore.Web.Core.Interfaces;
using Postcore.Web.Core.WebModels;
using Postcore.Web.Core.WebModels.Home;

namespace Postcore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IAdApiClient _adApiClient;

        public HomeController(
            ILogger<HomeController> logger,
            IMapper mapper,
            ISearchApiClient searchApiClient,
            IAdApiClient adApiClient)
        {
            _logger = logger;
            _mapper = mapper;
            _searchApiClient = searchApiClient;
            _adApiClient = adApiClient;
        }

        public IActionResult Index()
        {
            ViewBag.Current = "Home";
            _logger.LogInformation(nameof(HomeController));

            return View(new List<SearchViewModel>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string keyword)
        {
            ViewBag.Current = "Home";
            var searchResults = new List<SearchViewModel>();

            if (string.IsNullOrWhiteSpace(keyword))
                return View(searchResults);

            var ads = await _searchApiClient.Search(keyword).ConfigureAwait(false);
            ads.ForEach(ad => { searchResults.Add(new SearchViewModel(ad)); });

            return View(searchResults);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
