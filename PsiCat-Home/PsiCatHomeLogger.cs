namespace PsiCat.Home
{
    using System.Text;


    public class PsiCatHomeLogger : 
        PsiCat.ILogger,
        IConsoleContents
    {
        private readonly NLog.Logger nLogger;
        private StringBuilder logHistory = new StringBuilder();

        
        public PsiCatHomeLogger(NLog.Logger nLogger)
        {
            this.nLogger = nLogger;
        }


        public void LogDebug(string message)
        {
            this.nLogger.Log(NLog.LogLevel.Debug, message);
        }


        public void Log(string message)
        {
            this.nLogger.Log(NLog.LogLevel.Info, message);
            this.logHistory.AppendLine(message);
        }


        public void LogWarning(string message)
        {
            this.nLogger.Log(NLog.LogLevel.Warn, message);
        }


        public void LogError(string message)
        {
            this.nLogger.Log(NLog.LogLevel.Error, message);
        }


        public string ConsoleContent
        {
            get { return this.logHistory.ToString(); }
            set { }
        }
    }
}


