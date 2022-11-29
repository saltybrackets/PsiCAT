namespace PsiCat.Jira
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class JiraIssue
    {
        [JsonIgnore]
        public JiraRestClient JiraRestClient { get; set; }


        public string Id { get; set; }
        public string Self { get; set; }
        public string Key { get; set; }


        [JsonProperty("fields")]
        public JiraIssueData Data { get; set; } = new JiraIssueData();


        [JsonIgnore]
        public JiraUser Assignee
        {
            get { return this.Data.Assignee; }
        }


        [JsonIgnore]
        public JiraUser Creator
        {
            get { return this.Data.Creator; }
        }


        [JsonIgnore]
        public string Description
        {
            get { return this.Data.Description; }
        }


        [JsonIgnore]
        public string Summary
        {
            get { return this.Data.Summary; }
        }


        [JsonIgnore]
        public JiraUser Reporter
        {
            get { return this.Data.Reporter; }
        }


        public async Task<JiraResponse> SubmitComment(string commentText)
        {
            var body = new { body = commentText };

            HttpResponseMessage response = await this.JiraRestClient.Post(
                                               JiraEndpoint.ForComment(this.Key),
                                               body);

            if (!response.IsSuccessStatusCode)
                return null; // TODO

            string json = await response.Content.ReadAsStringAsync();
            JiraResponse jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(json);

            return jiraResponse;
        }


        public async Task<IEnumerable<JiraTransition>> GetPossibleTransitions()
        {
            HttpResponseMessage response =
                await this.JiraRestClient.Get(JiraEndpoint.ForTransitions(this.Key));
            if (!response.IsSuccessStatusCode)
                return null; // TODO

            string json = await response.Content.ReadAsStringAsync();
            JiraResponse jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(json);

            return jiraResponse.Transitions;
        }


        public async Task<string> GetTransitionId(string transitionName)
        {
            transitionName = transitionName.ToLower();
            IEnumerable<JiraTransition> possibleTransitions = await GetPossibleTransitions();
            JiraTransition jiraTransition = possibleTransitions
                .FirstOrDefault(transition => transition.Name.ToLower().Equals(transitionName));

            if (jiraTransition == null)
                return null; // TODO

            return jiraTransition.Id;
        }


        public async Task<JiraResponse> TransitionTo(string transitionName)
        {
            string transitionId = await GetTransitionId(transitionName);

            if (transitionId == null)
                return null; // TODO

            var body = new { transition = new { id = transitionId } };

            HttpResponseMessage response = await this.JiraRestClient.Post(
                                               JiraEndpoint.ForTransitions(this.Key),
                                               body);
            if (!response.IsSuccessStatusCode)
                return null; // TODO

            string json = await response.Content.ReadAsStringAsync();
            JiraResponse jiraResponse = JsonConvert.DeserializeObject<JiraResponse>(json);

            return jiraResponse;
        }
    }
}