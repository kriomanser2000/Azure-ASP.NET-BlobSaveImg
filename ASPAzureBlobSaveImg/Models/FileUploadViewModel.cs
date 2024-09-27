using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASPAzureBlobSaveImg.Models
{
    public class FileUploadViewModel
    {
        [Required]
        [Display(Name = "Choose file to upload")]
        public IFormFile File { get; set; }
    }
}
