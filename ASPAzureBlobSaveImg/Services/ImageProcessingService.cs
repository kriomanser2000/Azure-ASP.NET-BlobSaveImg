using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace ASPAzureBlobSaveImg.Services
{
    public class ImageProcessingService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ImageProcessingService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task CompressImageAsync(string blobName)
        {
            var incomeContainer = _blobServiceClient.GetBlobContainerClient("income");
            var outcomeContainer = _blobServiceClient.GetBlobContainerClient("outcome");
            var blobClient = incomeContainer.GetBlobClient(blobName);
            var outcomeBlobClient = outcomeContainer.GetBlobClient(blobName);
            using var imageStream = new MemoryStream();
            await blobClient.DownloadToAsync(imageStream);
            imageStream.Position = 0;
            using (var image = await Image.LoadAsync(imageStream))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 600),
                    Mode = ResizeMode.Max
                }));
                using var outputStream = new MemoryStream();
                await image.SaveAsJpegAsync(outputStream);
                outputStream.Position = 0;
                await outcomeBlobClient.UploadAsync(outputStream, overwrite: true);
            }
        }
    }
}