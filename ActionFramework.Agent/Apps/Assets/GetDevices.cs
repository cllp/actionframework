using System;
using Action = ActionFramework.Action;
using ActionFramework.Helpers.Data.Interface;
using ActionFramework.Helpers.Data;
using ActionFramework.Configuration;

namespace Assets
{
    public class GetDevices : Action
    {
        public string SenseConnectionString { get; set; }
        private IDataService _dataService;

        public override object Run(dynamic obj)
        {
            _dataService = DataFactory.GetDataService(ConfigurationManager.Settings["AgentSettings:AgentConnectionString"]);

            var result = _dataService.GetSingle<dynamic>("spGetDevices", null);

            try
            {
                return result;
            }
            catch (Exception ex)
            {
                //LogFactory.File.Error(ex, $"Action Search caused an exception");
                throw ex;
            }
        }

    }
}
