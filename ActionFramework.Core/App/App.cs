using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActionFramework.Core.App
{
    public abstract class App
    {
        //public abstract Guid AppId { get; }
        public abstract string Description { get; }
        public abstract List<Action> Actions { get; }

        public string AppName
        {
            get 
            { 
                return this.GetType().Name;
            }
        } 

        public string AppVersion
        {
            get 
            {
                return this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
            }
        } 

        //todo(?): Version of ActionFramework.dll that this app was referencing when built
    }
}
