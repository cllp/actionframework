using System;
using ActionFramework.Api;

namespace helloworld
{
    public class InAndOut : ActionFramework.Action
    {
        public override object Run(dynamic obj)
        {
            Log(new
            {
                InAndOutLog = obj.ToString()
            });

            return obj;
        }
    }
}

