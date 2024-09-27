using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using System;
using ASPAzureBlobSaveImg.Models;
using ASPAzureBlobSaveImg.Data;

namespace ASPAzureBlobSaveImg.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly MyAzureDb _context;
        private readonly string _containerName;
        public BlobController(BlobServiceClient blobServiceClient, IConfiguration configuration, MyAzureDb context)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = configuration["AzureBlobSettings:ContainerName"];
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }
            string url = blobClient.Uri.ToString();
            var imageRecord = new ImageRecord
            {
                FileName = fileName,
                Url = url
            };
            _context.ImageRecords.Add(imageRecord);
            await _context.SaveChangesAsync();
            return Ok(new { FileName = fileName, Url = url });
        }
    }
}