using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ActionFramework.Agent
{
    public class Agent
    {
        private string agentUri;

        public Agent(string remoteAgentUrl = "")
        {
            if (string.IsNullOrWhiteSpace(remoteAgentUrl))
            {
                if (!AgentIsAvailable())
                {
                    throw new Exception("Local agent is not available.");
                }

                agentUri = ""; //TODO: get from agent configuration
            }

            //TODO: check if remote agent is available
            // use System.Net.Http.HttpClient();
           
        }

        //TODO: Methods that should be available and/or a generic method that makes it possible to access any method in the Agent API

        private bool AgentIsAvailable()
        {
            return Assembly.GetEntryAssembly().GetName().Name == "ActionFrameworkAgent";
        }

    }
}
