﻿namespace Music_Library_Management_Application.Models.DbModels
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string? PlaylistTitle { get; set; }
        public string? PlaylistDescription { get; set; }
        public ICollection<SongPlaylist> SongPlaylists { get; set; }
    }
}
