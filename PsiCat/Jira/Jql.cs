namespace PsiCat.Jira
{
    using System.Collections.Generic;


    /// <summary>
    /// Effectively a StringBuilder for building up JQL queries, and then outputting a string.
    /// </summary>
    public class Jql
    {
        private readonly string separator = " ";
        private readonly List<string> terms = new List<string>();


        public Jql And()
        {
            this.terms.Add("AND");
            return this;
        }


        public Jql And(params Jql[] jqlQueries)
        {
            
        }
        

        public Jql Project
        {
            get
            {
                this.terms.Add("project");
                return this;
            }
        }


        public Jql FixVersion
        {
            get
            {
                this.terms.Add("fixVersion");
                return this;
            }
        }


        public Jql Status
        {
            get
            {
                this.terms.Add("status");
                return this;
            }
        }


        public Jql Created
        {
            get
            {
                this.terms.Add("created");
                return this;
            }
        }


        public Jql Assignee
        {
            get
            {
                this.terms.Add("assignee");
                return this;
            }
        }


        public Jql Type
        {
            get
            {
                this.terms.Add("type");
                return this;
            }
        }


        public Jql IssueType
        {
            get
            {
                this.terms.Add("issueType");
                return this;
            }
        }


        public Jql Issue
        {
            get
            {
                this.terms.Add("issue");
                return this;
            }
        }


        public Jql EqualTo(string value)
        {
            this.terms.Add($"={Escape(value)}");
            return this;
        }


        public Jql NotEqualTo(string value)
        {
            this.terms.Add($"!={Escape(value)}");
            return this;
        }


        public Jql GreaterThan(object value)
        {
            this.terms.Add($">{Escape(value.ToString())}");
            return this;
        }


        public Jql LessThan(object value)
        {
            this.terms.Add($"<{Escape(value.ToString())}");
            return this;
        }


        public static implicit operator string(Jql jql)
        {
            return jql.ToString();
        }


        /// <summary>
        /// Returns a completed JQL statement using all provided terms.
        /// </summary>
        /// <returns>JQL statement string.</returns>
        public override string ToString()
        {
            string final = $"({string.Join($" {this.separator} ", this.terms)})";
            return final;
        }


        private static string Escape(string value)
        {
            return value.Contains(' ')
                       ? $"\"{value}\""
                       : value;
        }
    }
}