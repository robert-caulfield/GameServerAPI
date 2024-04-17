using GameServerAPI.Models;
using System.Collections.Concurrent;

namespace GameServerAPI.Data
{
    /// <summary>
    /// Provides an in-memory store for game servers, allowing for the management of propeties
    /// defined in <see cref="GameServerCache"/>
    /// </summary>
    public class GameServerMemoryStore
    {
        /// <summary>
        /// Thread safe dictionary that maps game server ids to a GameServerCache object
        /// </summary>
        public ConcurrentDictionary<string, GameServerCache> ServerCache { get; } = new ConcurrentDictionary<string, GameServerCache>();


        /// <summary>
        /// Updates the cache of a game server. If one does not exist, one is created
        /// </summary>
        /// <param name="serverId">The Id of the game server</param>
        /// <param name="updateAction">Action that defines how the game server is updated</param>
        public void UpdateServerCache(string serverId, Action<GameServerCache> updateAction)
        {
            if(!ServerCache.TryGetValue(serverId, out var cache))
            { // If cache does not exist, create new one and apply update
                cache = new GameServerCache();
                updateAction(cache);
                ServerCache.TryAdd(serverId, cache);
            }
            else
            { // If cache exists just apply update
                updateAction(cache);
            }

        }

        /// <summary>
        /// Gets a game server cache object given associated with the given Id
        /// </summary>
        /// <param name="serverId">The Id of the game server</param>
        /// <returns></returns>
        public GameServerCache GetServerCache(string serverId)
        {
            return ServerCache.GetOrAdd(serverId, new GameServerCache());
        }

        /// <summary>
        /// Removes GameServerCache associated with given Id
        /// </summary>
        /// <param name="serverId">The Id of the game server</param>
        public void RemoveServerCache(string serverId)
        {
            ServerCache.TryRemove(serverId, out _);
        }
    }
}
