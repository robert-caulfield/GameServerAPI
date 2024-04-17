using GameServerAPI.Data;
using GameServerAPI.Models;
using GameServerAPI.Repository.IRepository;

namespace GameServerAPI.Repository
{
    public class GameServerRepository : Repository<GameServer>, IGameServerRepository
    {
        private readonly ApplicationDBContext _db;
        public GameServerRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
    }
}
