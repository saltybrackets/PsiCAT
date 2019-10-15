namespace PsiCat
{
    using System;
    using PsiCat.Git;
    using PsiCat.Jenkins;
    using PsiCat.Jira;


    [Serializable]
    public class PsiCatConfig : Config
    {
        public const string DefaultFilePath = "psicat-config.json";

        public GitConfig Git = new GitConfig();
        public JenkinsConfig Jenkins = new JenkinsConfig();
        public JiraConfig Jira = new JiraConfig();
        
        public string PluginsPath = "plugins";
    }
}