using Azure.Data.Tables;
using System.Diagnostics;
using System.Windows.Input;

namespace DailyLog.ViewModels
{
    public class DailyLogViewModel
    {
        private const string PartitionKey = "Key";
        private readonly TableClient _tableClient;

        public RadioButton Coffea { get; set; } = new RadioButton() { GroupName = "Coffea", Selection = 0 };
        public RadioButton Sauna { get; set; } = new RadioButton() { GroupName = "Sauna", Selection = 0 };

        //public string Status { get; set; }

        public ICommand SaveCommand { private set; get; }

        public DailyLogViewModel()
        {
            _tableClient = new TableClient("DefaultEndpointsProtocol=https;AccountName=dailylogstorage;AccountKey=SbalUV4skdA3fxsfIond8qA6VCGWDtF8UVTTtB5hIZGN0jpHzmTgEL9zb+pl0YNkTTPateLz9atx+AStHzSS9g==;EndpointSuffix=core.windows.net", "DailyLog");
            _tableClient.CreateIfNotExists();

            SaveCommand = new Command(async () =>
            {
                var entity = new TableEntity(PartitionKey, DateTime.Now.ToString("yyyyMMdd"))
                {
                    { "Coffea", Coffea.Selection },
                    { "Sauna", Sauna.Selection },
                };

                try
                {
                    await _tableClient.UpsertEntityAsync(entity);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Upsert Log entity failed: ", e.Message);
                }
            });
        }
    }
}
