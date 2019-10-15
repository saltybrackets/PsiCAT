namespace PsiCat.Jira
{
    using System;


    public class JiraIssue
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Human readable issue ID.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// URL to view the issue.
        /// </summary>
        public string Url { get; set; }
    }
}