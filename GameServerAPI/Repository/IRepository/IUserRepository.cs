using GameServerAPI.Models;

namespace GameServerAPI.Repository.IRepository
{
    /// <summary>
    /// Represents a repository for managing <see cref="ApplicationUser"/> objects.
    /// </summary>
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        // Future extension
    }
}
