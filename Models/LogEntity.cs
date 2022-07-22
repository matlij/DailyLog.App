using Azure;
using Azure.Data.Tables;

namespace DailyLog.Models
{
    public class LogEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public TraningType Training { get; set; }
        public int Health { get; set; }
        public int Coffea { get; set; }
        public int Sauna { get; set; }
    }
}
