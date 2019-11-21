using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Helpers.Data.Interface
{
    public interface ITableService
    {
        Task<bool> InsertMany<T>(IEnumerable<T> messages, string table, string partitionKey);

        Task<dynamic> InsertSingle<T>(T message, string tableName, string partitionKey, string rowKey, bool forInsert = true);

        dynamic Select(string tableName, string partitionKey, string rowKey, List<string> columns);
    }
}
