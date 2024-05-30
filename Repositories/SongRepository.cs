using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Repositories
{
    public class SongRepository : Repository<Song>, ISongRepository
    {
        public SongRepository(MyDbContext context) : base(context) {}

     
    }
}
