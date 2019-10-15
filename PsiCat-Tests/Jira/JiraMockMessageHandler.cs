namespace PsiCat.Tests.Jira
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using PsiCat.Jira;


    public class JiraMockMessageHandler : HttpMessageHandler
    {
        private Dictionary<string, Func<HttpRequestMessage, HttpResponseMessage>> requestHandlers =
            new Dictionary<string, Func<HttpRequestMessage, HttpResponseMessage>>
                {
                    { "POST:" + JiraEndpoint.Session, OnPostSession },
                    { "GET:" + JiraEndpoint.Search, OnGetSearch}
                };


        private static HttpResponseMessage OnGetSearch(HttpRequestMessage httpRequestMessage)
        {
            throw new NotImplementedException();
        }


        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }


        private static HttpResponseMessage OnPostSession(HttpRequestMessage httpRequestMessage)
        {
            throw new NotImplementedException();
        }
    }
}