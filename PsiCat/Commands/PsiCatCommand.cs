namespace PsiCat
{
    using System.Collections.Generic;


    public abstract class PsiCatCommand
    {
        public abstract string Name { get; }
        public abstract IEnumerable<string> Arguments { get; }
        
        public CommandHost CommandHost { get; set; }

        public abstract string Execute();
    }
}