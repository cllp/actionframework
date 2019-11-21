using System;
using Action = ActionFramework.Action;
using ActionFramework.Logger;
using ActionFramework.Helpers.Data.Interface;
using System.Collections.Generic;
using ActionFramework.Helpers.Data;
using ActionFramework.Configuration;

namespace Assets
{
    public class GetDailyReport : Action
    {
        public string SenseConnectionString { get; set; }
        private IDataService _dataService;

        public override object Run(dynamic obj)
        {
            try
            {
                var apiKey = ConfigurationManager.Settings["AgentSettings:SendGridAPiKey"];

                var sendMail = new ActionFramework.Helpers.Messaging.SendMail(apiKey, "claes-philip@staiger.se");
                var results = sendMail.Send(
                    "claes-philip@staiger.se", "Test Subject", "TEst COntent", "<b>Test content</b>");

                _dataService = DataFactory.GetDataService(ConfigurationManager.Settings["AgentSettings:AgentConnectionString"]);

                //get realestates
                var realestates = _dataService.GetMany<dynamic>("SELECT * FROM RealEstate");

                var resultObject = new List<dynamic>();

                foreach (var realEstate in realestates)
                {
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("RealEstateId", realEstate.RealEstateId.ToString());

                    resultObject.Add(new {
                        RealEstate = realEstate,
                        //Devices = _dataService.GetMany<dynamic>("spGetDevicesByRealEstate", parameters),
                        MessageStatistics = _dataService.GetMany<dynamic>("spGetYesterdayMessageCount", parameters)
                    });
                }
            
                return new {
                    Date = DateTime.Now.AddDays(-1).ToShortDateString(),
                    Statistics = resultObject
                };
            }
            catch (Exception ex)
            {
                //LogFactory.File.Error(ex, $"Action Search caused an exception");
                throw ex;
            }
        }

    }
}
