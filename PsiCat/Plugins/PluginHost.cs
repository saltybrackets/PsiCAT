namespace PsiCat.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;


    /// <summary>
    /// Manages a collection of plugins and syncs them with PsiCAT context.
    /// </summary>
    public class PluginHost
    {
        private Dictionary<string, PsiCatPlugin> pluginCollection;


        public PluginHost()
        {
            this.pluginCollection = new Dictionary<string, PsiCatPlugin>();
        }
        
        
        public ILogger Logger { get; set; }

        
        public Dictionary<string, PsiCatPlugin>.ValueCollection Collection
        {
            get { return this.pluginCollection.Values; }
        }
        

        public PsiCatPlugin GetPlugin(string key)
        {
            return this.pluginCollection[key];
        }


        /// <summary>
        /// Find all DLL's in specified directory and load available plugins.
        /// </summary>
        /// <param name="path">Path containing plugin DLL's.</param>
        /// <param name="commandHost">ComandHost that will house plugins Commands.</param>
        public void LocatePlugins(string path, CommandHost commandHost)
        {
            this.pluginCollection.Clear();

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (fileInfo.Extension.ToLower().Equals(".dll"))
                {
                    // Add any plugins from file if it contains a valid assembly.
                    try
                    {
                        Assembly pluginAssembly = LoadAssembly(file);
                        if (pluginAssembly != null
                            && TryAddPlugins(pluginAssembly)
                            && commandHost != null)
                        {
                            commandHost.LocateCommands(pluginAssembly);
                        }
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        if (this.Logger != null)
                            this.Logger.LogWarning($"Tried to load {fileInfo.Name}, but wasn't a plugin assembly.");
                    }
                }
            }
        }


        /// <summary>
        /// Adds any plugins found in an assembly to the plugin collection.
        /// </summary>
        /// <param name="pluginAssembly">Assembly which may contain plugins.</param>
        /// <returns>True if plugins were found in assembly, false if not.</returns>
        public bool TryAddPlugins(Assembly pluginAssembly)
        {
            bool foundPlugins = false;
            
            foreach (Type possiblePlugin in pluginAssembly.GetTypes())
            {
                // Skip if this isn't actually a plugin.
                if (!IsPlugin(possiblePlugin))
                    continue;

                // This is the droid we're looking for, instantiate it.
                PsiCatPlugin newPlugin = (PsiCatPlugin)Activator.CreateInstance(possiblePlugin);

                // Add the instance to the plugin collection if it's not a dupe.
                if (!this.pluginCollection.ContainsKey(newPlugin.Name))
                {
                    if (this.Logger != null)
                        this.Logger.LogInfo($"Found PsiCAT Plugin: {newPlugin.Name}.");
                    
                    foundPlugins = true;
                    newPlugin.PluginHost = this;
                    this.pluginCollection.Add(newPlugin.Name, newPlugin);
                }
                else
                {
                    if (this.Logger != null)
                        this.Logger.LogWarning($"Ignored duplicate plugin: {newPlugin.Name}.");
                }
            }
            
            return foundPlugins;
        }


        private static bool IsPlugin(Type possiblePlugin)
        {
            return
                possiblePlugin.IsSubclassOf(typeof(PsiCatPlugin))
                && possiblePlugin.IsInterface == false
                && possiblePlugin.IsAbstract == false;
        }


        private Assembly LoadAssembly(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch
            {
                return null;
            }
        }
    }
}