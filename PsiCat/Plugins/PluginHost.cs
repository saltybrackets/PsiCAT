namespace PsiCat.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;


    /// <summary>
    /// Manages a collection of plugins and syncs them with PsiCAT context.
    /// </summary>
    public class PluginHost : IEnumerable<PsiCatPlugin>
    {
        private Dictionary<string, PsiCatPlugin> pluginCollection;


        public PluginHost()
        {
            this.pluginCollection = new Dictionary<string, PsiCatPlugin>();
        }


        public PsiCatPlugin this[string key]
        {
            get { return GetPlugin(key); }
        }


        public IEnumerator<PsiCatPlugin> GetEnumerator()
        {
            return this.pluginCollection.Values.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public PsiCatPlugin GetPlugin(string key)
        {
            return this.pluginCollection[key];
        }


        /// <summary>
        /// Find all DLL's in specified directory and load available plugins.
        /// </summary>
        /// <param name="pluginsPath">Path containing plugin DLL's.</param>
        public void LocatePlugins(string path, CommandHost commandHost = null)
        {
            // logger.Info("Loading plugins...");
            this.pluginCollection.Clear();

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (fileInfo.Extension.Equals(".dll"))
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
                        // logger.Warning("Tried to load {0}, but wasn't a plugin assembly.", fileInfo.Name);
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
                    foundPlugins = true;
                    // logger.Info("Adding {0} to plugin collection.", newPlugin.Name);
                    newPlugin.PluginHost = this;
                    this.pluginCollection.Add(newPlugin.Name, newPlugin);
                    
                }
                else
                {
                    // logger.Warning("Tried to add duplicate {0} plugin to collection.", newPlugin.Name);
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