using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloWorld.Actions;
using ActionFramework.App;

namespace HelloWorld
{
    public class HelloWorld : App
    {
        public override string Description => "Hello world app";
       // public override Guid AppId => Guid.Parse("89f6005c-e8b4-45b4-bc45-f50a49288701");

        public override List<ActionFramework.App.Action> Actions => new List<ActionFramework.App.Action>
        {
            new SayHello(),
            new SayGoodbye()
        };
    }
}
