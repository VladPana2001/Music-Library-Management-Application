using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models.DbModels;

namespace Music_Library_Management_Application.Data
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<SongPlaylist> SongPlaylists { get; set; }
        public DbSet<MixDb> Mixes { get; set; }
        public DbSet<MixSongDb> MixSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-many relationship for User and Song
            builder.Entity<Song>()
                .HasOne(s => s.User)
                .WithMany(u => u.Songs)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Enable cascade delete

            // Configure one-to-many relationship for User and Playlist
            builder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Enable cascade delete

            // Configure many-to-many relationship for Song and Playlist
            builder.Entity<SongPlaylist>()
                .HasKey(sp => sp.Id); // Use Id as primary key

            builder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Song)
                .WithMany(s => s.SongPlaylists)
                .HasForeignKey(sp => sp.SongId)
                .OnDelete(DeleteBehavior.NoAction); // Enable cascade delete for song

            builder.Entity<SongPlaylist>()
                .HasOne(sp => sp.Playlist)
                .WithMany(p => p.SongPlaylists)
                .HasForeignKey(sp => sp.PlaylistId)
                .OnDelete(DeleteBehavior.NoAction); // Enable cascade delete for playlist
            // Configure one-to-many relationship for MixDb and MixSongDb
            builder.Entity<MixDb>()
                .HasMany(m => m.MixSongs)
                .WithOne(ms => ms.MixDb)
                .HasForeignKey(ms => ms.MixDbId)
                .OnDelete(DeleteBehavior.Cascade); // Enable cascade delete for mix songs

            // Configure one-to-many relationship for User and MixDb
            builder.Entity<MixDb>()
                .HasOne(m => m.User)
                .WithMany(u => u.Mixes)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Enable cascade delete for user mixes

        }
    }
}
