namespace PsiCat.Jira
{
    using System.Collections.Generic;


    public class JiraResponse
    {
        public string Expand { get; set; }
        public IEnumerable<JiraIssue> Issues { get; set; }
        public int MaxResults { get; set; }
        public int StartAt { get;set; }
        public int Total { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
        public int Status { get; set; }
        public IDictionary<string, string> Errors { get; set; }
        public IEnumerable<JiraTransition> Transitions { get; set; } 
    }
}