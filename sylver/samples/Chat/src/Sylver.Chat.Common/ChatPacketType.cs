namespace Sylver.Chat.Common
{
    public enum ChatPacketType
    {
        /// <summary>
        /// Sets the name of a client.
        /// </summary>
        SetName,

        /// <summary>
        /// Result of the SetName operation.
        /// </summary>
        SetNameResult,

        /// <summary>
        /// Sends a message to all server.
        /// </summary>
        Chat,

        /// <summary>
        /// Chat answer.
        /// </summary>
        ChatAnswer,

        Test,
        TestLargeFile,
    }
}
