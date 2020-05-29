namespace PsiCat.Jira
{
    using System.Collections.Generic;


    public class JiraUser
    {
        public IDictionary<string, string> AvatarUrls;
        public string DisplayName;
        public string EmailAddress;
        public bool IsActive;
        public string Key;
        public string Name;
        public string Self;
        public string TimeZone;
    }
}