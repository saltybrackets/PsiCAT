namespace PsiCat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    

    public class CommandHost : IEnumerable<PsiCatCommand>
    {
        private Dictionary<string, PsiCatCommand> commandCollection;


        public CommandHost()
        {
            this.commandCollection = new Dictionary<string, PsiCatCommand>();
        }


        public PsiCatCommand this[string key]
        {
            get { return GetCommand(key); }
        }


        public IEnumerator<PsiCatCommand> GetEnumerator()
        {
            return this.commandCollection.Values.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public PsiCatCommand GetCommand(string key)
        {
            return this.commandCollection[key];
        }
        
        
        public void LocateCommands(Assembly assembly)
        {
            foreach (Type possibleCommand in assembly.GetTypes())
            {
                // Skip if this isn't actually a command.
                if (!IsCommand(possibleCommand))
                    continue;

                // This is the droid we're looking for, instantiate it.
                PsiCatCommand newCommand = (PsiCatCommand)Activator.CreateInstance(possibleCommand);

                // Add the instance to the command collection if it's not a dupe.
                if (!this.commandCollection.ContainsKey(newCommand.Name))
                {
                    newCommand.CommandHost = this;
                    this.commandCollection.Add(
                        StandardizeCommandName(newCommand.Name), 
                        newCommand);
                }
                else
                {
                    // logger.Warning("Tried to add duplicate {0} command to collection.", newPlugin.Name);
                }
            }
        }


        private string StandardizeCommandName(string commandName)
        {
            return commandName
                .ToLower()
                .Trim()
                .Replace(" ", "-");
        }
        
        
        private static bool IsCommand(Type possibleCommand)
        {
            return
                possibleCommand.GetInterface(nameof(PsiCatCommand)) != null
                && possibleCommand.IsInterface == false
                && possibleCommand.IsAbstract == false;
        }
    }
}