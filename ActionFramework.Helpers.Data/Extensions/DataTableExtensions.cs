using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace ActionFramework.Helpers.Data.Extensions
{
    public static class DataTableExtensions
    {
        public static List<dynamic> AsDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }

        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            var json = JsonConvert.SerializeObject(data);//JsonSerializer.Serialize(data);
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            //OBS!!! TODO!!! Check if this datatable conversion works
            //DataTable dt = JsonSerializer.Deserialize<DataTable>(json);//(DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            return dt;
        }
    }
}
