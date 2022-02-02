using System;
using System.Collections.Generic;

namespace ActionFramework.Configuration
{
    public class ApiSettings
    {
        public bool Active { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public bool Authorize { get; set; }
    }

    public class ActionConfig
    {
        public class Property
        {
            public string Type { get; set; }
            public string Key { get; set; }
            public object Value { get; set; }
        }

        public string ActionName { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Cron { get; set; }
        public ApiSettings ApiSettings { get; set; }
        public List<Property> Properties { get; set; }

    }
}
