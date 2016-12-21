using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.ProjectModel;
using Newtonsoft.Json;

namespace ActionFrameworkAgent.Configuration
{
    //TODO: create a proper configuration provider https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration#custom-config-providers
    public class ConfigManager
    {
        public readonly string SettingsFilePath = ProjectRootResolver.ResolveRootDirectory(Directory.GetCurrentDirectory()) + "\\" + "agentsettings.json"; //todo: check if this works on different OS
        private const int DefaultPortNumber = 7406;

        public void InitConfiguration()
        {
            var applicationPath = ProjectRootResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());

            //create settings file if it doesn't exist
            if (!File.Exists(SettingsFilePath))  
            {
                var agentSettings = new AgentSettings {PortNumber = DefaultPortNumber};
                var json = JsonConvert.SerializeObject(agentSettings, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
        }
    }
}
