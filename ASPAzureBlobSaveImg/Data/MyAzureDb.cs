using Microsoft.EntityFrameworkCore;
using ASPAzureBlobSaveImg.Models;

namespace ASPAzureBlobSaveImg.Data
{
    public class MyAzureDb : DbContext
    {
        public MyAzureDb(DbContextOptions<MyAzureDb> options) : base(options) { }
        public DbSet<ImageRecord> ImageRecords { get; set; }
    }
}
