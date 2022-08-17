using Azure;
using Azure.Data.Tables;

namespace DailyLog.Models
{
    public class SurveyEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string CustomContent { get; set; }
    }
}
