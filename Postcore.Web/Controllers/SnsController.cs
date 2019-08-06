using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Postcore.Web.Core.Interfaces;
using Postcore.Web.Core.WebModels;
using Postcore.Web.Hubs;

namespace Postcore.Web.Controllers
{
    public class SnsController : Controller
    {
        private readonly ILogger<SnsController> _logger;
        private readonly HttpClient _client;
        private readonly IHostingEnvironment _env;
        private readonly IHubContext<SnsHub> _hubContext;
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;

        public SnsController(ILogger<SnsController> logger, 
            IHttpClientFactory clientFactory, 
            IHostingEnvironment env, 
            IHubContext<SnsHub> hubcontext,
            IFileUploader fileUploader,
            IConfiguration configuration)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _env = env;
            _hubContext = hubcontext;
            _fileUploader = fileUploader;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(nameof(SnsController));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                var id = DateTimeOffset.Now.ToString("yyyyMMddTHHmmss");
                var bucket = _configuration.GetValue<string>("RekognitionBucket");
                var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                var filePath = $"{id}/{fileName}";

                try
                {
                    using (var readStream = imageFile.OpenReadStream())
                    {
                        var result = await _fileUploader.UploadFileAsync(filePath, readStream, bucket)
                            .ConfigureAwait(false);
                        if (!result)
                            throw new Exception(
                                "Could not upload the image to file repository. Please see the logs for details.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _logger.LogError($"{nameof(SnsController)} : {e.Message}");

                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm()
        {
            string content;
            using (var reader = new StreamReader(Request.Body))
                content = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(content))
                return new BadRequestObjectResult($"{nameof(SnsController)}: SNS message is empty");

            SnsMessage snsMessage = null;
            String result = string.Empty;

            try
            {
                if (_env.IsDevelopment())
                {
                    snsMessage = JsonConvert.DeserializeObject<SnsMessage>(content);
                }
                else
                {
                    var message = Message.ParseMessage(content);

                    if (!message.IsSubscriptionType)
                    {
                        _logger.LogInformation($"{nameof(SnsController)}: SNS message received: " + message.MessageText);
                        snsMessage = JsonConvert.DeserializeObject<SnsMessage>(message.MessageText);
                        result = "Regular request";
                    }
                    else
                    {
                        var url = await _client.GetStringAsync(message.SubscribeURL);
                        _logger.LogInformation($"{nameof(SnsController)}: SNS SubscribeURL: " + url);
                        result = "Confirmed";
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.ToString());
            }

            if (snsMessage != null)
            {
                snsMessage.Body.ForEach(label =>
                {
                    _hubContext.Clients.All.SendAsync("ReceiveMessage", label);
                });
            }

            return new OkObjectResult(result);
        }
    }
}