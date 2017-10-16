using System.Reflection;

namespace ActionFramework.Core.Agent
{
    public class AgentInformation
    {
        public string OsDescription { get; }
        public string ActionFrameworkDescription { get; }
        public string AgentDescription { get; }
        //TODO
        //public string InternalIp { get; }



        public AgentInformation()
        {
            //TODO: values should only be set when in context of Agent. 

            OsDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            var assembly = Assembly.GetEntryAssembly(); 
            AgentDescription = assembly.FullName;

            ActionFrameworkDescription = typeof(ActionFramework.Core.App.App).AssemblyQualifiedName;
        }
    }
}
