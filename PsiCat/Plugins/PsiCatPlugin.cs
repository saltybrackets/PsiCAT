namespace PsiCat.Plugins
{
    public abstract class PsiCatPlugin
    {
        public abstract PluginHost PluginHost { get; set; }

        public ILogger Logger { get; set; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }
        public abstract Config Config { get; set; }


        public abstract void OnStart();


        public virtual void OnInitialize()
        {
        }


        public virtual void OnClose()
        {
        }


        public virtual void OnUpdate()
        {
        }
    }
}