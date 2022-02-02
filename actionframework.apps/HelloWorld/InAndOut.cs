using System;
using ActionFramework.Api;

namespace helloworld
{
    public class InAndOut : ActionFramework.Action
    {
        public override object Run(dynamic obj)
        {
           
            logger.Information("THIS IS A TEST");
            return obj;
        }
    }
}

