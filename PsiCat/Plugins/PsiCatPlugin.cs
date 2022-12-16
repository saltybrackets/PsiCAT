namespace PsiCat.Plugins
{
    public abstract class PsiCatPlugin
    {
        public abstract PluginHost PluginHost { get; set; }

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }
        public abstract Config Config { get; }


        public abstract void OnStart();

        public virtual ILogger Logger
        {
            get { return this.PluginHost.Logger; }
        }

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