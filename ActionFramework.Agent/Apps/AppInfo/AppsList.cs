using System;
using Action = ActionFramework.Action;
using System.Diagnostics;

namespace LLT.API.Device
{
    public class AppsList : Action
    {
        public override object Run(dynamic obj)
        {
            try
            {

                if (Log == null)
                {
                    //Console.WriteLine("Action Logger is null (Console)");
                    Trace.TraceWarning("Logger is null (Trace)");
                    Debug.WriteLine("Action Logger is null (Debug)");
                }
                else
                {
                    //Console.WriteLine("Action Logger is OK (Console)");
                    Trace.TraceInformation("Logger is ok (Trace)");

                    Log.Debug("Action Logger OK (Debug)");
                }

                /*
                using (logger.BeginScope(
                new Dictionary<string, object> { { "PersonId", 5 } }))
                            {
                                logger.LogInformation("Hello");
                                logger.LogInformation("World");
                            }
                */
                
                Log.Debug("DoSometing DEBUG message");
                Log.Information("DoSometing INFO message");
                Log.Warning("DoSometing WARN message");
                Log.Error("DoSometing ERROR message");
                Log.Fatal("DoSometing Fatal message");
                
                return "Ok";
                //return ActionFramework.AppContext.Current;
            }
            catch (Exception ex)
            {
                //LogFactory.File.Error(ex, $"AppsList caused an exception");
                throw ex;
            }
        }

    }
}
