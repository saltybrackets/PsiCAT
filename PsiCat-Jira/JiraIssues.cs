namespace PsiCat.Jira
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class JiraIssues
    {
        private JiraRestClient jiraRestClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
                                                          {
                                                              NullValueHandling =
                                                                  NullValueHandling.Ignore,
                                                              MissingMemberHandling =
                                                                  MissingMemberHandling.Ignore
                                                          };


        public JiraIssues(JiraRestClient jiraRestClient)
        {
            this.jiraRestClient = jiraRestClient;
        }


        public async Task<JiraIssue> Find(string issueKey)
        {
            IEnumerable<JiraIssue> foundIssues = await Find(new Jql().Issue.EqualTo(issueKey));
            return foundIssues.FirstOrDefault();
        }


        public async Task<IEnumerable<JiraIssue>> Find(Jql searchQuery)
        {
            var body = new
                           {
                               jql = searchQuery.ToString(),
                               startAt = 0
                           };
            HttpResponseMessage response =
                await this.jiraRestClient.Post(JiraEndpoint.Search, body);

            if (!response.IsSuccessStatusCode)
                return null; // TODO

            string json = await response.Content.ReadAsStringAsync();
            JiraResponse jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(json);
            IEnumerable<JiraIssue> jiraIssues = jiraResponse.Issues;
            foreach (JiraIssue jiraIssue in jiraIssues)
                jiraIssue.JiraRestClient = this.jiraRestClient;

            return jiraResponse.Issues;
        }


        public async Task<JiraResponse> SubmitComment(string issueKey, string commentText)
        {
            var body = new { body = commentText };

            HttpResponseMessage response = await this.jiraRestClient.Post(
                                               JiraEndpoint.ForComment(issueKey),
                                               body);

            if (!response.IsSuccessStatusCode)
                return null; // TODO

            string json = await response.Content.ReadAsStringAsync();
            JiraResponse jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(json);

            return jiraResponse;
        }
    }
}