using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Action = ActionFramework.App.Action;

namespace HelloWorld.Actions
{
    public class SayHello : Action
    {
        //public override string Name => "Say hello";
        public override string Description => "Say hello action description";
        //public override Guid ActionId => Guid.Parse("a749d90e-7ce9-4ee1-bfd9-67982c68f89d");

        public override bool Execute(out string actionMessage)
        {
            actionMessage = "Hello my dear friend!";
            return true;
        }
    }
}
