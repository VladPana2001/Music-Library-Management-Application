using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;

namespace Music_Library_Management_Application.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options){}

        public DbSet<MusicFile> MusicFiles { get; set; }
    }

    
}
