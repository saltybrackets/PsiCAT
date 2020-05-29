namespace PsiCat.Jira
{
    public static class JiraEndpoint
    {
        public const string Api = "/rest/api/2";
        public const string Auth = "/rest/auth/1";

        public const string Session = Auth + "/session";

        public const string Search = Api + "/search";

        public const string Issue = Api + "/issue";


        public static string ForTransitions(string issueKey)
        {
            return $"{Issue}/{issueKey}/transitions";
        }
        
        
        public static string ForComment(string issueKey)
        {
            return $"{Issue}/{issueKey}/comment";
        }
        
        
        public static string ForIssue(string issueKey)
        {
            return $"{Issue}/{issueKey}";
        }


        public static string ForIssue(JiraIssue jiraIssue)
        {
            return $"{Issue}/{jiraIssue.Key}";
        }
    }
}