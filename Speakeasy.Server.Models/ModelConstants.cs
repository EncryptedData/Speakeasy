namespace Speakeasy.Server.Models;

public static class ModelConstants
{
    public static class Limits
    {
        /// <summary>
        /// Maximum for most names
        /// </summary>
        public const int MaxNameLength = 128;

        /// <summary>
        /// The maximum number of messages you can fetch
        /// </summary>
        public const int MaxMessageFetchLimit = 100;

        /// <summary>
        /// The default number of message you can fetch
        /// </summary>
        public const int DefaultMessageFetchLimit = 10;

        /// <summary>
        /// The maximum message length
        /// </summary>
        public const int MaxMessageLength = 4096;
    }
}