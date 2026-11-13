namespace Mixtape.Shared
{
    // An error the user can see: its message is shown as it is.
    public sealed class AppException : Exception
    {
        public AppException(string message)
            : base(message)
        {
        }

        public AppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
