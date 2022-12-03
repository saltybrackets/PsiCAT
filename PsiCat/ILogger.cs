namespace PsiCat
{
    public interface ILogger
    {
        void Log(string message);
        void LogDebug(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}