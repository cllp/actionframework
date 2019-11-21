using System;
using Action = ActionFramework.Action;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ActionFramework.Helpers.Data.Interface;
using ActionFramework.Helpers.Data;
using ActionFramework.Configuration;

namespace Assets
{
    public class SyncDevices : Action
    {
        public string SenseConnectionString { get; set; }
        private IDataService _dataService;

        public override object Run(dynamic jsonData)
        {

            _dataService = DataFactory.GetDataService(ConfigurationManager.Settings["AgentSettings:AgentConnectionString"]);

            try
            {
                JArray array = JArray.Parse(jsonData.ToString());

                foreach (JObject obj in array.Children<JObject>())
                {
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("DevEui", obj["DevEui"].ToString());
                    parameters.Add("DeviceTypeId", obj["DeviceTypeId"].ToString());
                    parameters.Add("NetworkProvider", obj["NetworkProvider"].ToString());
                    parameters.Add("DeviceStatusId", obj["DeviceStatusId"].ToString());

                    var result = _dataService.Insert("spSyncDevices", parameters);
                }

                return true;
            }
            catch (Exception ex)
            {
                var errormsg = $"Action SyncDevices caused an exception. Input: {jsonData.ToString()}";
                Log.Error(ex, errormsg);
                throw ex;
            }
        }

    }
}
