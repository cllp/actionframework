using System;
using System.Collections.Generic;

namespace ActionFramework.Core.App
{
    public class IApp
    {
        public IApp()
        {
            
        }

        public string Description
        {
            get;
            set;
        }

        public List<Action> Actions
        {
            get;
            set;
        }

        public string AppName
        {
            get;
        }

        public string AppVersion
        {
            get;
        }
    }
}
