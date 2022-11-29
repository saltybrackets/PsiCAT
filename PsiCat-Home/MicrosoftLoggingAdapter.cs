namespace PsiCat.Home
{
    public partial class MicrosoftLoggingAdapter : PsiCat.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger adaptee;


        public MicrosoftLoggingAdapter(Microsoft.Extensions.Logging.ILogger adaptee)
        {
            this.adaptee = adaptee;
        }


        public void Log(string message)
        {
            this.adaptee.Log(LogLevel.Debug, message);
        }


        public void LogInfo(string message)
        {
            this.adaptee.Log(LogLevel.Information, message);
        }


        public void LogWarning(string message)
        {
            this.adaptee.Log(LogLevel.Warning, message);
        }


        public void LogError(string message)
        {
            this.adaptee.Log(LogLevel.Error, message);
        }
    }
}


