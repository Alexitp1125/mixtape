using Microsoft.Extensions.Logging;
using Mixtape.Shared;

namespace Mixtape.Application
{
    internal static class ExceptionExtensions
    {
        // Writes the real error to the log and returns the one the user can see.
        public static AppException Handle(this Exception exception, string message, ILogger logger)
        {
            logger.LogError(exception, "{Message}", message);
            return new AppException(message, exception);
        }
    }
}
