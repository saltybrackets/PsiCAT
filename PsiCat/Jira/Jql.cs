namespace PsiCat.Jira
{
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    /// Effectively a StringBuilder for building up JQL queries, and then outputting a string.
    /// </summary>
    public class Jql
    {
        private readonly string separator = " ";
        private readonly StringBuilder terms = new StringBuilder();


        public Jql Or
        {
            get { 
                this.terms.Append(" OR ");
                return this;
            }
        }
        
        public Jql And
        {
            get
            {
                this.terms.Append(" AND ");
                return this;
            }
        }


        public Jql Project
        {
            get
            {
                this.terms.Append("project");
                return this;
            }
        }


        public Jql FixVersion
        {
            get
            {
                this.terms.Append("fixVersion");
                return this;
            }
        }


        public Jql Status
        {
            get
            {
                this.terms.Append("status");
                return this;
            }
        }


        public Jql Created
        {
            get
            {
                this.terms.Append("created");
                return this;
            }
        }


        public Jql Assignee
        {
            get
            {
                this.terms.Append("assignee");
                return this;
            }
        }


        public Jql Type
        {
            get
            {
                this.terms.Append("type");
                return this;
            }
        }


        public Jql IssueType
        {
            get
            {
                this.terms.Append("issueType");
                return this;
            }
        }


        public Jql Issue
        {
            get
            {
                this.terms.Append("issue");
                return this;
            }
        }


        public Jql EqualTo(string value)
        {
            this.terms.Append($"={Escape(value)}");
            return this;
        }


        public Jql NotEqualTo(string value)
        {
            this.terms.Append($"!={Escape(value)}");
            return this;
        }


        public Jql GreaterThan(object value)
        {
            this.terms.Append($">{Escape(value.ToString())}");
            return this;
        }


        public Jql LessThan(object value)
        {
            this.terms.Append($"<{Escape(value.ToString())}");
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
            //string final = $"({string.Join($"{this.separator}", this.terms)})";
            //return final;
            return this.terms.ToString();
        }


        private static string Escape(string value)
        {
            return value.Contains(' ')
                       ? $"\"{value}\""
                       : value;
        }
    }
}