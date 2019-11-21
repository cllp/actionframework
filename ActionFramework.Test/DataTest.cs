using System;
using Xunit;
using System.Linq;
using System.Data;
using ActionFramework.Helpers.Data;
using System.Collections.Generic;
using ActionFramework.Helpers.Data.Interface;
using System.Dynamic;

namespace ActionFramework.Test
{
    public class DataTests
    {
        private IDataService _dataService = DataFactory.GetDataService("***");

        
        [Fact]
        public void InsertBulkMessages()
        {
            List<dynamic> objects = new List<dynamic>();

            for (int i = 0; i < 1000; i++)
            {
                objects.Add(new
                {
                    Name = i.ToString(),
                    Date = DateTime.Now
                });
            }

            var result = _dataService.InsertMany<dynamic>(objects, "Test");


            Assert.NotNull(result);
        }
       

        [Fact]
        public void TestDataRow()
        {
            var dt = new DataTable();
            dt.Columns.Add("DevEui", typeof(string));
            dt.Columns.Add("CommTimestamp", typeof(DateTime));

            dt.Rows.Add("123", "2017-01-01 00:00:00");
            dt.Rows.Add("456", "2018-01-01 00:00:00");
            dt.Rows.Add("789", "2019-01-01 00:00:00");

            var rowValue = (DateTime)dt.Select().OrderByDescending(u => u["CommTimestamp"]).ToArray().FirstOrDefault()["CommTimestamp"];

            Assert.True(rowValue > DateTime.Parse("2018-01-01 00:00:00"));

        }

        [Fact]
        public void TestInsertTableStorage()
        {
            string conn = "DefaultEndpointsProtocol=https;AccountName=lltstoragedev;AccountKey=DgM+O1vVgC4cUh3KNmnr7Zvy+1XTk5oPBFWSXrkrirXusnFYEeEpmh7VNR54afq8AHwibjBmu/+qR1CDZ/Afjw==;EndpointSuffix=core.windows.net";
            List<dynamic> objects = new List<dynamic>();

            for (int i = 0; i < 10; i++)
            {
                objects.Add(new {
                    Prop1 = i.ToString(),
                    Prop2 = DateTime.Now
                });
            }

            var _tableService = DataFactory.GetTableService(conn);
            _tableService.InsertMany<dynamic>(objects, "TestTable", System.Guid.NewGuid().ToString());

            
            Assert.True(true);
        }

        [Fact]
        public void TestInsertSingleTableStorage()
        {
            string conn = "DefaultEndpointsProtocol=https;AccountName=lltstoragedev;AccountKey=DgM+O1vVgC4cUh3KNmnr7Zvy+1XTk5oPBFWSXrkrirXusnFYEeEpmh7VNR54afq8AHwibjBmu/+qR1CDZ/Afjw==;EndpointSuffix=core.windows.net";

            var _tableService = DataFactory.GetTableService(conn);
            var tblobj = new
            {
                Prop1 = "Test",
                Prop2 = DateTime.Now
            };

            _tableService.InsertSingle<dynamic>(tblobj, "TestTable", System.Guid.NewGuid().ToString(), System.Guid.NewGuid().ToString());


            Assert.True(true);
        }


        [Fact]
        public void FindGaps()
        {
            List<int> listStringVals = (new int[] { 7, 13, 8, 12, 10, 11, 14 }).ToList();
            listStringVals.Sort();

            var result = listStringVals.Skip(1).Select((x, i) => x - listStringVals[i] == 1).Any(x => !x);

            Assert.True(true);
        }

        [Fact]
        public void FindGaps2()
        {

            List<dynamic> messages = new List<dynamic>
            {
                //new { FcntUp = 101, CommTimestamp = "2019-01-01 00:00:01" },
                //new { FcntUp = 102, CommTimestamp = "2019-01-01 00:00:02" },
                //new { FcntUp = 103, CommTimestamp = "2019-01-01 00:00:03" },
                new { FcntUp = 1, CommTimestamp = "2019-01-01 00:00:04" },
                new { FcntUp = 2, CommTimestamp = "2019-01-01 00:00:05" },
                new { FcntUp = 3, CommTimestamp = "2019-01-01 00:00:06" },
                new { FcntUp = 5, CommTimestamp = "2019-01-01 00:00:07" },
                new { FcntUp = 8, CommTimestamp = "2019-01-01 00:00:07" }

            };

            //look for number: 1 and if index is > 0 we should split the list

            var strings = messages.OrderBy(o => o.CommTimestamp).Select(o => o.FcntUp.ToString()).ToArray();

            var list = Array.ConvertAll(strings, s => Int32.Parse(s)).OrderBy(i => i).Cast<int>();

            int min = list.Min();
            int max = list.Max();

            var result = Enumerable.Range(min, max - min + 1).Except(list);

            Assert.True(true);
        }

        [Fact]
        public void SplitListONIndexRestart()
        {
            var arraylist = new List<List<dynamic>>();

            List<dynamic> messages = new List<dynamic>
            {
                new { FcntUp = 101, CommTimestamp = "2019-01-01 00:00:01" },
                new { FcntUp = 102, CommTimestamp = "2019-01-01 00:00:02" },
                new { FcntUp = 103, CommTimestamp = "2019-01-01 00:00:03" },

                //restart of sequence
                new { FcntUp = 1, CommTimestamp = "2019-01-01 00:00:04" },
                new { FcntUp = 2, CommTimestamp = "2019-01-01 00:00:05" },
                new { FcntUp = 3, CommTimestamp = "2019-01-01 00:00:06" },
                
                //this scenario = several restart of sequence -> FcntUp.
                new { FcntUp = 1, CommTimestamp = "2019-01-01 00:00:07" },
                new { FcntUp = 2, CommTimestamp = "2019-01-01 00:00:08" },
                new { FcntUp = 3, CommTimestamp = "2019-01-01 00:00:09" },
                new { FcntUp = 5, CommTimestamp = "2019-01-01 00:00:10" }
            };

            //group by FcntUp and CommTimestamp
            var query = messages.GroupBy(x => new { x.FcntUp, x.CommTimestamp });

            //declare the current item
            dynamic currentItem = null;

            //declare the list of ranges
            List<dynamic> range = null;

            //loop through the the sorted list
            foreach (var item in query)
            {
                //check if start of new range
                if (currentItem == null || item.Key.FcntUp < currentItem.Key.FcntUp)
                {
                    //create a new list if the FcntUp starts on a new range
                    range = new List<dynamic>();

                    //add the list to the parent list
                    arraylist.Add(range);
                }

                //add the item to the sublist
                range.Add(item);

                //set the current item
                currentItem = item;
            }

            Assert.True(arraylist.Count() == 3);
        }


    }

}
