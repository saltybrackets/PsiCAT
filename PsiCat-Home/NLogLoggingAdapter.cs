namespace PsiCat.Home
{
    public class NLogLoggingAdapter : PsiCat.ILogger
    {
        private readonly NLog.Logger adaptee;


        public NLogLoggingAdapter(NLog.Logger adaptee)
        {
            this.adaptee = adaptee;
        }


        public void LogDebug(string message)
        {
            this.adaptee.Log(NLog.LogLevel.Debug, message);
        }


        public void Log(string message)
        {
            this.adaptee.Log(NLog.LogLevel.Info, message);
        }


        public void LogWarning(string message)
        {
            this.adaptee.Log(NLog.LogLevel.Warn, message);
        }


        public void LogError(string message)
        {
            this.adaptee.Log(NLog.LogLevel.Error, message);
        }
    }
}


