namespace PsiCat.Jira
{
    using System.Collections.Generic;


    public class JiraProject
    {
        public IDictionary<string, string> AvatarUrls { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ProjectTypeKey { get; set; }
        public string Self { get; set; }
        
    }
}