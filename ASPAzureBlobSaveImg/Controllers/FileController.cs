using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using ASPAzureBlobSaveImg.Data;
using ASPAzureBlobSaveImg.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.IO;

namespace ASPAzureBlobSaveImg.Controllers
{
    public class FileController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueClient _queueClient;
        private readonly MyAzureDb _dbContext;
        public FileController(BlobServiceClient blobServiceClient, QueueClient queueClient, MyAzureDb dbContext)
        {
            _blobServiceClient = blobServiceClient;
            _queueClient = queueClient;
            _dbContext = dbContext;
        }
        [HttpGet]
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(FileUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("income");
                var blobClient = containerClient.GetBlobClient(model.File.FileName);
                using (var stream = model.File.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }
                await _queueClient.SendMessageAsync(model.File.FileName);
                var outcomeContainer = _blobServiceClient.GetBlobContainerClient("outcome");
                var compressedBlobClient = outcomeContainer.GetBlobClient(model.File.FileName);
                var compressedImageUrl = compressedBlobClient.Uri.ToString();
                _dbContext.Files.Add(new FileModel { FileName = model.File.FileName, CompressedImageUrl = compressedImageUrl });
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}