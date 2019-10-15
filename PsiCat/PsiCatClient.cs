namespace PsiCat
{
    using System;
    using System.IO;
    using System.Reflection;
    using PsiCat.Jira;
    using PsiCat.Plugins;


    /// <summary>
    /// Main PsiCAT hub.
    /// </summary>
    public class PsiCatClient
    {
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

        private JiraRestClient jiraRestClient;


        /// <summary>
        /// Main Jira client, using values from Core.Config.
        /// </summary>
        public JiraRestClient JiraRest
        {
            get
            {
                if (jiraRestClient == null)
                    jiraRestClient = new JiraRestClient(Config.Jira);
                return jiraRestClient;
            }
            set { jiraRestClient = value; }
        }


        /// <summary>
        /// Indicates when all components are loaded, and plugins may begin working.
        /// </summary>
        public void Start()
        {
            foreach (PsiCatPlugin plugin in Plugins)
                plugin.OnStart();
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
                Config = PsiCat.Config.LoadFromJson<PsiCatConfig>(path);
            }
            else
            {
                Config = new PsiCatConfig();
                Config.Save(path);
            }
        }


        /// <summary>
        /// Load all native PsiCAT commands.
        /// </summary>
        public void LoadInternalCommands()
        {
            Commands = new CommandHost();
            Assembly internalAssembly = Assembly.GetCallingAssembly();
            Commands.LocateCommands(internalAssembly);
        }


        /// <summary>
        /// Load all available plugins from the plugins directory (set in config).
        /// If no plugins directory found, it will be created.
        /// </summary>
        public void LoadPlugins()
        {
            Plugins = new PluginHost();

            if (Directory.Exists(Config.PluginsPath))
            {
                Console.Out.WriteLine(Directory.GetCurrentDirectory());
                Plugins.LocatePlugins(Config.PluginsPath, Commands);
                foreach (PsiCatPlugin plugin in Plugins)
                    plugin.OnInitialize();
            }
            else
            {
                Directory.CreateDirectory(Config.PluginsPath);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            foreach (PsiCatPlugin plugin in Plugins)
                plugin.OnClose();
        }
    }
}