namespace PsiCat.Jira
{
    public static class JiraEndpoint
    {
        public const string Api = "/rest/api/2";
        public const string Auth = "/rest/auth/1";

        public const string Session = Auth + "/session";

        public const string Search = Api + "/search";
    }
}