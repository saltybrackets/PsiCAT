namespace PsiCat.Jira
{
    using System.Threading.Tasks;


    public class JiraIssues
    {
        private JiraRestClient jiraRestClient;


        public JiraIssues(JiraRestClient jiraRestClient)
        {
            this.jiraRestClient = jiraRestClient;
        }


        public async Task<JiraIssue> Find(Jql searchQuery)
        {
            
        }
    }
}