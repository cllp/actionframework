using System;
using System.Net.Http;
using System.Threading.Tasks;
using ActionFramework;
using Xunit;
using System.Linq;
using ActionFramework.Logger;

namespace ActionFramework.Test
{
    public class ActionTests
    {
        [Fact]
        public void FindAction()
        {
            var sayhello = AppContext.Action("SayHello");
            Assert.NotNull(sayhello);
        }

        public void FindApp()
        {
            var sayhello = AppContext.App("HelloWorld");
            Assert.NotNull(sayhello);
        }

        [Fact]
        public void RunAction()
        {
            var sayhello = AppContext.Action("SayHello");
            var result = sayhello.Run(new { Input = "This is what I put in" });
            Assert.NotNull(result);
        }

        [Fact]
        public void RunActionWithLog()
        {
            var sayhello = AppContext.Action("SayHello");
            var result = sayhello.Run(new { Input = "This is what I put in" });
            LogFactory.File.Information($"Action {sayhello.ActionName} log: { result }");

            foreach (var log in sayhello.Logs)
            {
                LogFactory.File.Information(log.ToString());
            }

            Assert.NotNull(result);
        }
    }
}
