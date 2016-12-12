using System.Reflection;

namespace ActionFrameworkAgent
{
    public class SystemInformation
    {
        public string OsDescription { get; }
        public string ActionFrameworkDescription { get; }
        public string AgentDescription { get; }


        public SystemInformation()
        {
            OsDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            var assembly = Assembly.GetEntryAssembly(); 
            AgentDescription = assembly.FullName;

            ActionFrameworkDescription = typeof(ActionFramework.App.App).AssemblyQualifiedName;
        }
    }
}
