namespace PsiCat
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            PsiCatClient psiCatClient = new PsiCatClient();
            psiCatClient.LoadConfig();
            psiCatClient.LoadInternalCommands();
            psiCatClient.LoadPlugins();
            psiCatClient.Start();
            psiCatClient.Close();
        }
    }
}