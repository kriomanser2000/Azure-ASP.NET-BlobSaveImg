using System;
using System.ComponentModel.DataAnnotations;

namespace ASPAzureBlobSaveImg.Models
{
    public class FileModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        public string CompressedImageUrl { get; set; }
        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        [Required]
        public string UserId { get; set; }
    }
}
