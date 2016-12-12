using System.Reflection;

namespace ActionFrameworkAgent
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
            OsDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            var assembly = Assembly.GetEntryAssembly(); 
            AgentDescription = assembly.FullName;

            ActionFrameworkDescription = typeof(ActionFramework.App.App).AssemblyQualifiedName;
        }
    }
}
