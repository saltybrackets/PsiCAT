using NUnit.Framework;


namespace PsiCat.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using PsiCat.Jira;


    public class Tests
    {
        private PsiCatClient psiCatClient;
        
        [SetUp]
        public void Setup()
        {
            this.psiCatClient = new PsiCatClient();
            psiCatClient.LoadConfig();
            psiCatClient.LoadInternalCommands();
            psiCatClient.LoadPlugins();
            psiCatClient.Start();
            Console.Out.WriteLine("Test setup completed.");
        }


        [Test]
        public async Task QuickTest()
        {
            //await CreateNewIssue();
            //await FindTest();
            //await CommentOnIssue();
            await TransitionIssue();
        }


        private async Task CommentOnIssue()
        {
            JiraIssue issue = await this.psiCatClient.JiraRest
                                  .Issues
                                  .Find("TST-1");
            var response = await issue.SubmitComment("This is a test comment.");
            Console.Out.WriteLine(JsonConvert.SerializeObject(response));
        }


        public async Task FindTest()
        {
            IEnumerable<JiraIssue> issues = await this.psiCatClient.JiraRest
                .Issues
                .Find(new Jql()
                    .Project.EqualTo("Test Project")
                    .Or
                    .Issue.EqualTo("TST-2"));
            foreach (var issue in issues)
            {
                //Console.Out.WriteLine(JsonConvert.SerializeObject(issue));
                Console.Out.WriteLine(issue.Data.Assignee.Name);
            }
        }


        public async Task CreateNewIssue()
        {
            JiraIssue newIssue = new JiraIssue();
            newIssue.Assignee.Name = "TestAssigneeName";
            
            Console.Out.WriteLine(JsonConvert.SerializeObject(newIssue));
        }


        public async Task TransitionIssue()
        {
            JiraIssue issue = await this.psiCatClient.JiraRest
                                  .Issues
                                  .Find("TST-1");
            var response = await issue.TransitionTo("In Review");
            Console.Out.WriteLine(JsonConvert.SerializeObject(response));
        }
    }
}