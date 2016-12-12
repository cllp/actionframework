using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ActionFramework.Agent.App;

namespace ActionFramework.Agent
{
    public static class Agent
    {
        public static AppRepository GetAppRepository()
        {
            if (!AgentIsAvailable())
            {
                return null;
            }

            return new AppRepository();
        }

        public static bool AgentIsAvailable()
        {
            return Assembly.GetEntryAssembly().GetName().Name == "ActionFrameworkAgent";
        }

    }
}
