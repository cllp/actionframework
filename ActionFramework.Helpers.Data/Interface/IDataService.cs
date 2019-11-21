using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ActionFramework.Helpers.Data.Interface
{
    public interface IDataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        IEnumerable<T> GetMany<T>(string sql);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        IEnumerable<T> GetMany<T>(string procedure, IDictionary<string, string> parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T GetSingle<T>(string procedure, IDictionary<string, string> parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        T GetSingle<T>(string sql);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        string Insert(string procedure, IDictionary<string, string> parameterList);

        string Insert<T>(string procedure, IDictionary<string, string> parameterList, List<T> data, string tableValueParamaterName, string tableValueParamaterType);

        bool InsertMany<T>(List<T> data, string destinationTableName, int batchSize = 100);

        bool InsertMany(DataTable data, string destinationTableName, int batchSize = 100);
    }
}
