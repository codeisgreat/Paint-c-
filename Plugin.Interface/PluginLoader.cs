using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;

namespace Plugin.Interface
{
    public class PluginLoader
    {
        /// <summary>
        /// Loads the plugins from current folder
        /// [All files ending with Plugin.dll are supposed to have IPlugin implementations]
        /// </summary>
        /// <returns></returns>
        public IList<IPlugin> LoadPlugins(string folder)
        {
            IList<IPlugin> plugins = new List<IPlugin>();

            // Get files in folder
            string[] files = Directory.GetFiles(folder, "*Plugin.dll");
            foreach (string file in files)
            {
                Assembly assembly = Assembly.LoadFile(file);
                var types = assembly.GetExportedTypes();

                foreach (Type type in types)
                    if (type.GetInterfaces().Contains(typeof(IPlugin)))
                    {
                        object instance = Activator.CreateInstance(type);
                        plugins.Add(instance as IPlugin);
                    }
            }

            return plugins;
        }
    }
}
