namespace SchoolApp.Services.Interfaces
{
    public interface IAppLogger
    {
        void LogInfo(string message);

        void LogError(string message);

        // ✅ NEW OVERLOAD: accepts the actual Exception object
        // WHY: The old LogError(string) only logged a custom message like
        //      "An error occurred while saving the student."
        //      That tells you NOTHING useful. You don't know:
        //        - What the real error was
        //        - Which line it happened on
        //        - What the stack trace looks like
        //
        //      By passing the Exception, Serilog will log:
        //        - The exception TYPE  (e.g. SqlException, NullReferenceException)
        //        - The exception MESSAGE (e.g. "Cannot insert null into column 'Name'")
        //        - The full STACK TRACE (exact file and line number)
        //
        //      This makes debugging 10x faster.
        void LogError(string message, Exception ex);
    }
}