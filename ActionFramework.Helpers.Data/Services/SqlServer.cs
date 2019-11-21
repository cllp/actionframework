using ActionFramework.Helpers.Data.Extensions;
using ActionFramework.Helpers.Data.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ActionFramework.Helpers.Data
{

    internal class SqlServer : IDataService
    {
        private readonly string _connectionString;

        public SqlServer(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal IDbConnection Connection => new SqlConnection(_connectionString);

        public IEnumerable<T> GetMany<T>(string sql)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                try
                {
                    var result = dbConnection.Query<T>(sql);
                    return result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IEnumerable<T> GetMany<T>(string procedure, IDictionary<string, string> parameters)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                try
                {
                    var queryParameters = new DynamicParameters();

                    foreach (var par in parameters)
                    {
                        queryParameters.Add("@" + par.Key, par.Value);
                    }

                    var result = dbConnection.Query<T>(procedure, queryParameters, commandType: CommandType.StoredProcedure);
                    return result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public T GetSingle<T>(string procedure, IDictionary<string, string> parameters)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                dynamic result = null;

                try
                {
                    if (parameters != null)
                    { 
                        var queryParameters = new DynamicParameters();

                        foreach (var par in parameters)
                        {
                            queryParameters.Add("@" + par.Key, par.Value);
                        }

                        result = dbConnection.Query<T>(procedure, queryParameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    }
                    else
                        result = dbConnection.Query<T>(procedure, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    return result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public T GetSingle<T>(string sql)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                try
                {
                    var result = dbConnection.Query<T>(sql).FirstOrDefault();
                    return result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string Insert(string procedure, IDictionary<string, string> parameterList)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                try
                {
                    var queryParameters = new DynamicParameters();

                    foreach (var par in parameterList)
                    {
                        queryParameters.Add("@" + par.Key, par.Value);
                    }

                    var result = dbConnection.Execute(procedure, queryParameters, commandType: CommandType.StoredProcedure);

                    return "Ok";

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Insert batch data as parameter with stored procedure
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="parameterList"></param>
        /// <param name="data"></param>
        /// <param name="tableValueParamaterName"></param>
        /// <param name="tableValueParamaterType"></param>
        /// <returns></returns>
        public string Insert<T>(string procedure, IDictionary<string, string> parameterList, List<T> data, string tableValueParamaterName, string tableValueParamaterType)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                try
                {
                    var queryParameters = new DynamicParameters();

                    foreach (var par in parameterList)
                    {
                        queryParameters.Add("@" + par.Key, par.Value);
                    }

                    //queryParameters.Add("@Keys", tableValueParamater.AsTableValuedParameter("dbo.BulkMessagesTableType"));
                    queryParameters.Add("@" + tableValueParamaterName, data.AsDataTable().AsTableValuedParameter(tableValueParamaterType));

                    var result = dbConnection.Execute(procedure, queryParameters, commandType: CommandType.StoredProcedure);

                    return "Ok";

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool InsertMany<T>(List<T> data, string destinationTableName, int batchSize = 100)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                //using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = destinationTableName;
                    try
                    {
                        var dt = data.AsDataTable();
                        bulkCopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        connection.Close();
                        throw ex;
                    }
                }

                transaction.Commit();
            }

            return true;

        }

        public bool InsertMany(DataTable data, string destinationTableName, int batchSize = 100)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = destinationTableName;
                    try
                    {
                        bulkCopy.WriteToServer(data);
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        connection.Close();
                        throw ex;
                    }
                }

                transaction.Commit();
            }

            return true;

        }
    }
}
