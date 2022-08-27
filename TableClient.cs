using Azure.Data.Tables;

namespace DailyLog
{
    public class TableClient<ITableEntity>
    {
        public TableClient Client { get; }

        public TableClient(string connectionString, string tableName)
        {
            Client = new TableClient(connectionString, tableName);
            Client.CreateIfNotExists();
        }
    }
}
