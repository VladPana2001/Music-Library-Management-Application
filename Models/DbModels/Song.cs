﻿namespace Music_Library_Management_Application.Models.DbModels
{
    public class Song
    {
        public int SongId { get; set; }
        public string? SongTitle { get; set; }
        public string? Description { get; set; }
        public string? SongArtist { get; set; }
        public string? SongAlbum { get; set; }
        public string? SongLenght { get; set; }
        public short? SongBPM { get; set; }
        public byte[]? SongFile { get; set; }

        public ICollection<SongPlaylist> SongPlaylists { get; set; }

    }
}