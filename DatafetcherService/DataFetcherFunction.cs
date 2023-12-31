using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftAzureDataFetcherFunction.DatafetcherService
{
    public class DataFetcherFunction
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("DataFetcherFunction")]
        public static async Task Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, // Runs every 5 minutes
            ILogger log)
        {
            log.LogInformation($"DataFetcherFunction executed at: {DateTime.Now}");

            string apiUrl = "https://api.example.com/data";

            string data = await FetchDataFromAPI(apiUrl);

            await StoreDataInBlobStorage(data);
        }

        private static async Task<string> FetchDataFromAPI(string apiUrl)
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task StoreDataInBlobStorage(string data)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("YourStorageConnectionString");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("data-container");
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("data.json");
            await blockBlob.UploadTextAsync(data);
        }
    }
}
