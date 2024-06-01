﻿namespace Music_Library_Management_Application.Repositories.Interfaces
{
    public interface IRepositoryWrapper
    {
        ISongRepository Songs { get; }
        IPlaylistRepository Playlists { get; }
    }
}