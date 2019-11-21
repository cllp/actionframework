using ActionFramework.Helpers.Data.Interface;
using Microsoft.OData.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Helpers.Data.Services
{
    internal class TableStorage : ITableService
    {
        private string _connectionString;
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;

        public TableStorage(string connectionString)
        {
            _connectionString = connectionString;
            storageAccount = CloudStorageAccount.Parse(_connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public async Task<bool> InsertMany<T>(IEnumerable<T> messages, string table, string partitionKey)
        {
            //get the table reference
            CloudTable messageTable = tableClient.GetTableReference(table);

            //create the table if not exist
            await messageTable.CreateIfNotExistsAsync();

            //declare the list of dynamic entities
            List<DynamicTableEntity> dynamicEntities = new List<DynamicTableEntity>();

            //loop through the messages
            foreach (var msg in messages)
            {
                //get type
                Type t = msg.GetType();
                //get the properties
                PropertyInfo[] properties = t.GetProperties();

                //create dictionary and values
                //declare the dictionary
                var tblprop = new Dictionary<string, EntityProperty>();

                foreach (var prop in properties)
                {
                    tblprop.Add(prop.Name, new EntityProperty(prop.GetValue(msg).ToString()));
                }

                //create the rowkey
                var rowKey = System.Guid.NewGuid().ToString();

                //create a dynamic entity
                var entity = new DynamicTableEntity(partitionKey, rowKey, "*", tblprop);

                dynamicEntities.Add(entity);
            }

            //split the dynamic entities list in batches if > 100
            var batches = SplitList<DynamicTableEntity>(dynamicEntities, 100);

            foreach (var list in batches)
            {
                TableBatchOperation batchOperation = new TableBatchOperation();
                foreach (var device in list)
                {
                    //device.PartitionKey = partitionKey;//masterBatchGuid.ToString();
                    batchOperation.Insert(device);
                }

                // Execute the batch operation.
                IList<TableResult> executeresult = await messageTable.ExecuteBatchAsync(batchOperation);
            }

            return true;
        }

        public async Task<dynamic> InsertSingle<T>(T message, string tableName, string partitionKey, string rowKey, bool forInsert = true)
        {
            //get the table reference
            CloudTable messageTable = tableClient.GetTableReference(tableName);

            //create the table if not exist
            await messageTable.CreateIfNotExistsAsync();

            //get type
            Type t = message.GetType();
            //get the properties
            PropertyInfo[] properties = t.GetProperties();

            //create dictionary and values
            //declare the dictionary
            var tblprop = new Dictionary<string, EntityProperty>();

            foreach (var prop in properties)
            {
                tblprop.Add(prop.Name, new EntityProperty(prop.GetValue(message).ToString()));
            }

            //create a dynamic entity
            var entity = new DynamicTableEntity(partitionKey, rowKey, "*", tblprop);

            try
            {
                TableResult result;

                if (forInsert)
                {
                    var insertOperation = TableOperation.Insert(entity);
                    result = await messageTable.ExecuteAsync(insertOperation);
                }
                else
                {
                    var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);
                    result = await messageTable.ExecuteAsync(insertOrMergeOperation);
                }

                return result;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public IEnumerable<List<T>> SplitList<T>(List<T> items, int nSize = 30)
        {
            for (int i = 0; i < items.Count; i += nSize)
            {
                yield return items.GetRange(i, Math.Min(nSize, items.Count - i));
            }
        }

        public dynamic Select(string tableName, string partitionKey, string rowKey, List<string> columns)
        {
            CloudTable table = tableClient.GetTableReference(tableName);

            //TableResult result;

            var selectOperation = TableOperation.Retrieve(partitionKey, rowKey, columns);
            var result = table.ExecuteAsync(selectOperation);


            var tblprop = new Dictionary<string, EntityProperty>();

            //tblprop.Add(prop.Name, new EntityProperty(prop.GetValue(message).ToString()));

            //create a dynamic entity
            //var entity = new DynamicTableEntity(partitionKey, rowKey, "*", tblprop);
            
            /*
            DataServiceContext ctx = new DataServiceContext();
            var query = (from entity in ctx.CreateQuery<DynamicTableEntity>(tableName)
                        where entity.PartitionKey == "MyPartitionKey"
                        select new { entity.RowKey });

            IEnumerable<DynamicTableEntity> query = from entity in
                                       ctx.CreateQuery<DynamicTableEntity>(tableName)
                                              where entity.PartitionKey == "MyPartitionKey"
                                              select new DynamicTableEntity
                                              {
                                                  PartitionKey = entity.PartitionKey,
                                                  RowKey = entity.RowKey,
                                                  A = entity.A,
                                                  D = entity.D,
                                                  I = entity.I
                                              };

            */

            return result.Result;
        }
    }
}
