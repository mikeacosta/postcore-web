using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Postcore.Web.Core.Interfaces;
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
        private readonly ISnsAdapter _snsAdapter;

        public SnsController(ILogger<SnsController> logger, 
            IHttpClientFactory clientFactory, 
            IHostingEnvironment env, 
            IHubContext<SnsHub> hubcontext,
            IFileUploader fileUploader,
            IConfiguration configuration,
            ISnsAdapter snsAdapter)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _env = env;
            _hubContext = hubcontext;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _snsAdapter = snsAdapter;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(nameof(SnsController));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile != null && IsImage(imageFile.FileName))
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

            String snsMessage = string.Empty;
            String result = string.Empty;

            try
            {
                var message = Message.ParseMessage(content);

                if (!message.IsSubscriptionType)
                {
                    _logger.LogInformation($"{nameof(SnsController)}: SNS message received: " + message.MessageText);
                    SendSnsMessages(message.MessageText);
                    result = "Regular request";
                }
                else
                {
                    var url = await _client.GetStringAsync(message.SubscribeURL);
                    _logger.LogInformation($"{nameof(SnsController)}: SNS SubscribeURL: " + url);
                    result = "Confirmed";
                }
            }
            catch (Exception e)
            {
                if (e != null)
                    _logger.LogError($"{nameof(SnsController)} error: {e.Message}");

                return new BadRequestObjectResult(e.ToString());
            }

            _logger.LogInformation($"{nameof(SnsController)} result: {result}");
            return new OkObjectResult(result);
        }

        private void SendSnsMessages(string messages)
        {
            _snsAdapter.ToList(messages).ForEach(async label =>
            {
                _logger.LogInformation($"{nameof(SnsController)} sending message: {label}");
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", label);
            });
        }

        private bool IsImage(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            var extension = Path.GetExtension(file);
            return ImageMimeDictionary.ContainsKey(extension.ToLower());
        }

        private readonly IDictionary<string, string> ImageMimeDictionary = new Dictionary<string, string>
        {
            { ".bmp", "image/bmp" },
            { ".dib", "image/bmp" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".jpe", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" },
            { ".pnz", "image/png" }
        };
    }
}