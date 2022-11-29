namespace PsiCat
{
    using System;
    using System.IO;
    using System.Reflection;
    using PsiCat.Git;
    using PsiCat.Jenkins;


    [Serializable]
    public class PsiCatConfig : Config
    {
        public static readonly string DefaultFilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}"
                                                        + "/psicat-config.json";

        public GitConfig Git = new GitConfig();
        public JenkinsConfig Jenkins = new JenkinsConfig();
    }
}