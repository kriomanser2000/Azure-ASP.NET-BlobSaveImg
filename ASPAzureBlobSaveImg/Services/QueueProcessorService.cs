using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Queues;
using ASPAzureBlobSaveImg.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASPAzureBlobSaveImg.Services
{
    public class QueueProcessorService : BackgroundService
    {
        private readonly QueueClient _queueClient;
        private readonly ImageProcessingService _imageProcessingService;
        public QueueProcessorService(QueueClient queueClient, ImageProcessingService imageProcessingService)
        {
            _queueClient = queueClient;
            _imageProcessingService = imageProcessingService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueClient.ReceiveMessageAsync();
                if (message.Value != null)
                {
                    var blobName = message.Value.MessageText;
                    await _imageProcessingService.CompressImageAsync(blobName);
                    await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}