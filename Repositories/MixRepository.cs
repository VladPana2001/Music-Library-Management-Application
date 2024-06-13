using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;

namespace Music_Library_Management_Application.Repositories
{
    public class MixRepository : Repository<MixDb>, IMixRepository
    {
        public MixRepository(MyDbContext context) : base(context)
        {
        }

    }
}
