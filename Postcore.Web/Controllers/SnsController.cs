using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Postcore.Web.Controllers
{
    public class SnsController : Controller
    {
        private readonly ILogger<SnsController> _logger;
        private readonly HttpClient _client;

        public SnsController(ILogger<SnsController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            _logger.LogInformation("SnsController, dude!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm()
        {
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            var message = Message.ParseMessage(content);

            try
            {
                if (!message.IsSubscriptionType)
                    return new OkObjectResult("Regular request"); // handle the action

                var result = await _client.GetStringAsync(message.SubscribeURL);
                _logger.LogInformation("SNS SubscribeURL: " + message.SubscribeURL);
                _logger.LogInformation("SNS SubscribeURL: " + result);
                return new OkObjectResult("Confirmed");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.ToString());
            }
        }
    }
}