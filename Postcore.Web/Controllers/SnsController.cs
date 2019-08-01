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
            _logger.LogInformation(nameof(SnsController));
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

            if (string.IsNullOrWhiteSpace(content))
            {
                var msg = $"{nameof(SnsController)}: SNS message is empty";
                _logger.LogError(msg);
                return new BadRequestObjectResult(msg);
            }

            var message = Message.ParseMessage(content);

            try
            {
                if (!message.IsSubscriptionType)
                {
                    // TODO: handle message
                    _logger.LogInformation($"{nameof(SnsController)}: SNS message received: " + message.MessageText);
                    _logger.LogInformation($"{nameof(SnsController)}: SNS message: " + message.ToString());
                    return new OkObjectResult("Regular request");
                }

                var result = await _client.GetStringAsync(message.SubscribeURL);
                _logger.LogInformation($"{nameof(SnsController)}: SNS SubscribeURL: " + message.SubscribeURL);
                _logger.LogInformation($"{nameof(SnsController)}: SNS SubscribeURL: " + result);
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