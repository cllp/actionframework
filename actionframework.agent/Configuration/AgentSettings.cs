using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agent.Configuration
{
    /// <summary>
    /// Agent settings. -strongly typed settings
    /// </summary>
    public class AgentSettings
    {
        public string AgentName { get; set; }

        public string Environment { get; set; }

        public string AgentGuid { get; set; }

        public string ActionLogTable { get; set; }

        public string ActionDataTable { get; set; }

        public string Url { get; set; }

        public string AppsDir { get; set; }

        public bool UseHangfireServer { get; set; }

        public bool UseHangfireDashboard { get; set; }

        public string AgentConnectionString { get; set; }

        public string TableStorageConnectionstring { get; set; }

        public string Secret { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
