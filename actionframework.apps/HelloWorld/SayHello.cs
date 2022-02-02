using System;
using ActionFramework.Api;

namespace helloworld
{
    public class SayHello : ActionFramework.Action
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }


        public override object Run(dynamic obj)
        {
            //make a log

            //var my = Prop1;
            //var ny = Prop(obj, "Address");

            //ActionLogger.
            /*
            Log(new
            {
                LogType = "Info",
                Amount = 108,
                Message = "Hej hej hej"
            });
            */
            /*
            Log(new
            {
                InputObject = obj
            });
            */


            //Log(args[0]);

            logger.Information("Prop1" + Prop1);

            logger.Information("This is a test log");

            return new
            {
                P1 = Prop1,
                P2 = Prop2,
                inputobj = obj
            };
        }
    }
}
