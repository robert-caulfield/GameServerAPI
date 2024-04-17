namespace GameServerAPI.Configuration
{
    /// <summary>
    /// Configuration settings for the management of game servers. These
    /// settings control the heartbeat mechanisms of registered game servers.
    /// </summary>
    /// <remarks>
    /// Settings are loaded from "GameServerManagerSettings" section of appsettings file.
    /// </remarks>
    public class GameServerManagerSettings
    {
        /// <summary>
        /// Indicates whether the heartbeat mechanism is enabled. If true registered
        /// game servers are required to send periodic heartbeat signals via 
        /// API requests to indicate that they are active.
        /// </summary>
        public bool HeartbeatEnabled {  get; set; }
        /// <summary>
        /// The time in seconds that a server is removed if no heartbeat is recieved. 
        /// </summary>
        public int HeartbeatTimeout { get; set; }

        /// <summary>
        /// The interval in time in seconds that GameServers are periodically checked to
        /// see if their last heartbeat exceeeds <see cref="HeartbeatTimeout"/>
        /// </summary>
        public int HeartbeatCheckInterval { get; set; }
    }
}
