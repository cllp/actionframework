using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Action = ActionFramework.App.Action;

namespace HelloWorld.Actions
{
    public class SayGoodbye : Action
    {
        public override string Description => "Say goodbye action description";
        //public override Guid ActionId => Guid.Parse("db97d73c-235f-4056-8177-a5d1ae3c243d");

        public override bool Execute(out string actionMessage)
        {
            actionMessage = "Goodbye!";
            return true; 
        }
    }
}
