namespace PsiCat.Jenkins
{
    using System;
    
    
    [Serializable]
    public class JenkinsConfig : Config
    {
        public string ServerAddress;
        public string LoginUserName;
        public string LoginPassword;
    }
}