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

        /// <summary>
        /// The amount of UTF-16 characters that can combine into a single Emoji
        /// </summary>
        public const int EmojiCharacterStringLength = 4;

        /// <summary>
        /// The maximum amount of Reactions allowed on a given Message
        /// </summary>
        public const int MaxMessageReactions = 32;

        /// <summary>
        /// The maximum number of tags you can assign to entities that support tags
        /// </summary>
        public const int MaxNumberOfTags = 32;
    }
}