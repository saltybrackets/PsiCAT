namespace PsiCat.Jira
{
    public class JiraComment
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public JiraUser Author { get; set; }
        public string Body { get; set; }
        public JiraUser UpdateAuthor { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public JiraVisibility Visibility { get; set; } = new JiraVisibility();
    }
}