using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Data
{
    public class MyDbContext : IdentityDbContext<IdentityUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options){}

        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<SongPlaylist> SongPlaylists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SongPlaylist>()
                .HasKey(sp => new { sp.SongId, sp.PlaylistId });

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Song)
                .WithMany(s => s.SongPlaylists)
                .HasForeignKey(sp => sp.SongId);

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Playlist)
                .WithMany(p => p.SongPlaylists)
                .HasForeignKey(sp => sp.PlaylistId);
        }
    }
}
