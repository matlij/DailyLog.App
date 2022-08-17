using Azure;
using Azure.Data.Tables;

namespace DailyLog.Models
{
    public class LogValueEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public int Selection { get; set; }
    }
}
