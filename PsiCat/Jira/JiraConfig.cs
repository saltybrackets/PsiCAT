namespace PsiCat.Jira
{
    using System;


    [Serializable]
    public class JiraConfig : Config
    {
        public string BaseUrl;
        public string LoginUserName;
        public string LoginPassword;
    }
}