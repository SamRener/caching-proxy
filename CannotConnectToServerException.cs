
namespace CachingProxy
{
    [Serializable]
    internal class CannotConnectToServerException : Exception
    {
        public CannotConnectToServerException() : base("Failed to connect to server.")
        {
        }

        public CannotConnectToServerException(string? message) : base(message)
        {
        }

        public CannotConnectToServerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}