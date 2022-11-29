namespace PsiCat.Jira
{
    using Newtonsoft.Json;


    public class JiraIssueData
    {
        public JiraUser Assignee { get; set; } = new JiraUser();
        public string Created { get; set; }
        public JiraUser Creator { get; set; } = new JiraUser();
        public string Description { get; set; }
        public string DueDate { get; set; }
        public string Environment { get; set; }
        


        [JsonProperty(PropertyName = "issuetype")]
        public JiraIssueType IssueType { get; set; } = new JiraIssueType();


        
        public string LastViewed { get; set; }
        public JiraPriority Priority { get; set; } = new JiraPriority();
        public JiraProject Project { get; set; } = new JiraProject();
        public JiraUser Reporter { get; set; } = new JiraUser();
        
        public JiraStatusCategory StatusCategory { get; set; } = new JiraStatusCategory();
        public string Summary { get; set; }
        public string Updated { get; set; }
        public string Url { get; set; }
    }
}