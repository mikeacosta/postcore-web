using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Postcore.Web.Core.ApiModels;
using Postcore.Web.Core.Interfaces;
using Postcore.Web.Core.WebModels.AdManagement;

namespace Postcore.Web.Controllers
{
    [Authorize]
    public class AdManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdApiClient _client;
        private readonly IMapper _mapper;

        public AdManagementController(IFileUploader fileUploader, IAdApiClient client, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _client = client;
            _mapper = mapper;
        }

        public IActionResult Create(CreateAdViewModel model)
        {
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var ad = _mapper.ToAd(model);
                var createResponse = await _client.CreateAsync(ad);
                var id = createResponse.Id;
                var filePath = string.Empty;
 
                if (imageFile != null)
                {
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                .ConfigureAwait(false);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please see the logs for details.");
                        }

                        var isConfirmed = await _client.ConfirmAsync(new ConfirmRequest {
                            Id = id,
                            FilePath = filePath,
                            Status = AdApi.Shared.Models.AdStatus.Active
                        });

                        if (!isConfirmed)
                            throw new Exception($"Cannot confirm ad id = {id}");

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        await _client.ConfirmAsync(new ConfirmRequest {
                            Id = id,
                            FilePath = filePath,
                            Status = AdApi.Shared.Models.AdStatus.Pending
                        });

                        Console.WriteLine(e.Message);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}