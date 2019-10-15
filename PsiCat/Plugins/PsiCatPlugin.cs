namespace PsiCat.Plugins
{
    public abstract class PsiCatPlugin
    {
        public abstract PluginHost PluginHost { get; set; }

        public abstract Config Config { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }

        
        
        public abstract void OnStart();


        public virtual void OnInitialize()
        {
        }


        public virtual void OnClose()
        {
        }
    }
}