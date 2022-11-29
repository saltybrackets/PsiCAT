namespace PsiCat.Jira
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;


    public class JiraRestClient
    {
        public JiraIssues Issues;
        
        private JiraConfig config;
        private HttpClient httpClient;


        public JiraRestClient(JiraConfig config, HttpMessageHandler httpMessageHandler = null)
        {
            this.config = config;
            this.Issues = new JiraIssues(this);
            
            this.httpClient = httpMessageHandler != null
                                  ? new HttpClient(httpMessageHandler)
                                  : new HttpClient();

            string mergedCredentials = $"{config.LoginUserName}:{config.LoginPassword}";
            byte[] byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            string encodedCredentials = Convert.ToBase64String(byteCredentials);
            AuthenticationHeaderValue authenticationHeader = new AuthenticationHeaderValue(
                "Basic",
                encodedCredentials);

            this.httpClient.BaseAddress = new Uri(config.BaseUrl);
            this.httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        

        public async Task<HttpResponseMessage> Get(
            string endpoint, 
            NameValueCollection queryParameters = null)
        {
            /* TODO
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            StringBuilder encodedParameters = new StringBuilder();
            */
            
            
            HttpResponseMessage response = await this.httpClient.GetAsync(endpoint);
            return response;
        }
        
        
        public async Task<HttpResponseMessage> Post<T>(string endpoint, T body)
        {
            HttpResponseMessage response = await this.httpClient.PostAsJsonAsync(endpoint, body);
            return response;
        }
        
        
        
    }
}