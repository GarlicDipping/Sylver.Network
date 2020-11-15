namespace Sylver.Network.Client
{
    /// <summary>
    /// Provide properties to configure the <see cref="NetClient"/> retry strategy when the connection is lost.
    /// </summary>
    public class NetClientRetryConfiguration
    {
        /// <summary>
        /// Gets or sets how the client handles failed connections.
        /// When using <see cref="NetClientRetryOption.Limited"/> set <see cref="MaxAttempts"/>
        /// </summary>
        public NetClientRetryOption Mode { get; }

        /// <summary>
        /// Gets or sets the maximum number of times the client will try to reconnect to the server
        /// </summary>
        public int MaxAttempts { get; }

        /// <summary>
        /// Creates a new <see cref="NetClientRetryConfiguration"/>.
        /// </summary>
        /// <param name="mode">Retry mode.</param>
        /// <param name="maxAttempts">Maximum retry attempts if more is defined as <see cref="NetClientRetryOption.Limited"/>.</param>
        public NetClientRetryConfiguration(NetClientRetryOption mode, int maxAttempts)
        {
            Mode = mode;
            MaxAttempts = maxAttempts;
        }
    }
}
