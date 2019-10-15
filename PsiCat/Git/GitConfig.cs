namespace PsiCat.Git
{
    using System;
    
    
    [Serializable]
    public class GitConfig : Config
    {
        public string GitServerApiUrl;
        public string LoginUserName;
        public string LoginPassword;
        public string SharedSecret;
    }
}