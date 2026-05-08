namespace SchoolApp.Services.Implementations
{
    public class AppLogger : IAppLogger
    {
        private readonly ILogger<AppLogger> _logger;

        public AppLogger(ILogger<AppLogger> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        // ✅ NEW: passes the real Exception to Serilog
        // Serilog knows how to serialize Exception objects — it will write
        // the full type, message, and stack trace to your log file automatically.
        // The {message} part is YOUR context ("while saving student"),
        // the {ex} part is what C# actually threw.
        public void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
            //              ↑
            //       Note: ex comes FIRST in LogError(ex, message)
            //       This is the correct Serilog/ILogger signature.
            //       Many beginners accidentally write LogError(message, ex)
            //       which compiles but treats ex as a format argument, not
            //       as the exception to log — so the stack trace is LOST.
        }
    }
}