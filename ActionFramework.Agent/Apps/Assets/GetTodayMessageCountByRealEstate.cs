using System;
using Action = ActionFramework.Action;
using ActionFramework.Helpers.Data.Interface;
using System.Collections.Generic;
using ActionFramework.Helpers.Data;
using ActionFramework.Configuration;

namespace Assets
{
    public class GetTodayMessageCountByRealEstate : Action
    {
        public string SenseConnectionString { get; set; }
        private IDataService _dataService;

        public override object Run(dynamic obj)
        {
            try
            {
                _dataService = DataFactory.GetDataService(ConfigurationManager.Settings["AgentSettings:AgentConnectionString"]);

                var parameters = new Dictionary<string, string>();
                parameters.Add("RealEstateId", obj);

                //get realestates
                var realestate = _dataService.GetSingle<dynamic>($"SELECT * FROM RealEstate WHERE RealEstateId = {obj}");

                var result = _dataService.GetMany<dynamic>("spGetTodayMessageCountByRealEstate", parameters);
                return new {
                    RealEstate = realestate,
                    Devices = result
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"GetTodayMessageCountByRealEstate caused an exception");
                throw ex;
            }
        }

    }
}
