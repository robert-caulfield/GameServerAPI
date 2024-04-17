using GameServerAPI.Data;
using GameServerAPI.Models;
using GameServerAPI.Repository.IRepository;

namespace GameServerAPI.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDBContext _db;
        public UserRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

    }
}
