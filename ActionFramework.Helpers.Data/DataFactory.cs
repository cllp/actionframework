using ActionFramework.Helpers.Data.Interface;
using ActionFramework.Helpers.Data.Services;

namespace ActionFramework.Helpers.Data
{
    public static class DataFactory
    {
        public static IDataService GetDataService(string connectionString)
        {
            return new SqlServer(connectionString);
        }

        public static ITableService GetTableService(string connectionString)
        {
            return new TableStorage(connectionString);
        }
    }
}
    