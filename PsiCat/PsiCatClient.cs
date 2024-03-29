namespace PsiCat
{
    using System;
    using System.IO;
    using System.Reflection;
    using PsiCat.Plugins;


    /// <summary>
    /// Main PsiCAT hub.
    /// </summary>
    public class PsiCatClient
    {
        public static readonly string PluginsPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}";

        /// <summary>
        /// Logger that will be used across plugin host and child plugins.
        /// </summary>
        public ILogger Logger;

        /// <summary>
        /// Main PsiCAT and plugins config.
        /// </summary>
        public PsiCatConfig Config;

        /// <summary>
        /// Plugin host service and collection.
        /// </summary>
        public PluginHost Plugins;

        /// <summary>
        /// Commands collections.
        /// </summary>
        public CommandHost Commands;


        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            foreach (PsiCatPlugin plugin in this.Plugins.Collection)
            {
                plugin.OnClose();
            }
        }


        /// <summary>
        /// Load in main config file.
        /// If no config file found, a new one will be created.
        /// </summary>
        public void LoadConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = PsiCatConfig.DefaultFilePath;
            
            if (File.Exists(path))
            {
                this.Config = PsiCat.Config.LoadFromJson<PsiCatConfig>(path);
            }
            else
            {
                this.Logger.LogWarning($"Creating new config at: {path}");
                this.Config = new PsiCatConfig();
                this.Config.Save(path);
            }
        }


        /// <summary>
        /// Load all native PsiCAT commands.
        /// </summary>
        public void LoadInternalCommands()
        {
            this.Commands = new CommandHost();
            Assembly internalAssembly = Assembly.GetCallingAssembly();
            this.Commands.LocateCommands(internalAssembly);
        }


        /// <summary>
        /// Load all available plugins from the plugins directory (set in config).
        /// If no plugins directory found, it will be created.
        /// </summary>
        public void LoadPlugins()
        {
            this.Logger.Log($"Loading plugins at: {PluginsPath}...");
            
            this.Plugins = new PluginHost(this);

            if (Directory.Exists(PluginsPath))
            {
                this.Plugins.LocatePlugins(PluginsPath, this.Commands);
                foreach (PsiCatPlugin plugin in this.Plugins.Collection)
                {
                    this.Logger.Log($"Initializing plugin: {plugin.Name}");
                    plugin.PluginHost = this.Plugins;
                    plugin.OnInitialize();
                }
            }
            else
            {
                this.Logger.LogWarning($"Creating new plugins directory at: {PluginsPath}");
                Directory.CreateDirectory(PluginsPath);
            }
        }


        /// <summary>
        /// Indicates when all components are loaded, and plugins may begin working.
        /// </summary>
        public void Start()
        {
            foreach (PsiCatPlugin plugin in this.Plugins.Collection)
            {
                plugin.OnStart();
            }
        }


        public void Update()
        {
            foreach (PsiCatPlugin plugin in this.Plugins.Collection)
            {
                plugin.OnUpdate();
            }
        }
    }
}