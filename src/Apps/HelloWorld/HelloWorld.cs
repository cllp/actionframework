using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActionFramework.App;
using HelloWorld.Actions;
using Action = ActionFramework.App.Action;

namespace HelloWorld
{
    public class HelloWorld : App
    {
        public override string Description => "Hello world app";
       // public override Guid AppId => Guid.Parse("89f6005c-e8b4-45b4-bc45-f50a49288701");

        public override List<Action> Actions => new List<Action>
        {
            new SayHello(),
            new SayGoodbye()
        };
    }
}
