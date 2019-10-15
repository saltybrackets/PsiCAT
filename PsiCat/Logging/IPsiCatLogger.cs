namespace PsiCat.Logging
{
    public interface IPsiCatLogger
    {
        void LogTrace(string message);
        void Log(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogFatal(string message);
    }
}