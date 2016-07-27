using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace VsoBotStorage
{
    public class Token
    {
        CloudBlobContainer container;
        public Token()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            container = blobClient.GetContainerReference("tokens");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

        }
        public async Task<BotInterfaceApi.Models.TokenModel> GetToken(string userName)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(userName + ".json");
            if (!blockBlob.Exists()) return null;
            blockBlob.Properties.ContentType = "application/json";
            var tokenModel = await blockBlob.DownloadTextAsync();

            if (string.IsNullOrEmpty(tokenModel))
            {
                return JsonConvert.DeserializeObject<BotInterfaceApi.Models.TokenModel>(tokenModel);
            }
            return null;
        }
        public async Task SetToken(string userName, BotInterfaceApi.Models.TokenModel tokenModel)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(userName + ".json");
            blockBlob.Properties.ContentType = "application/json";
          
            if (tokenModel != null)
            {
               var stringfyTokenModel = JsonConvert.SerializeObject(tokenModel);
               await blockBlob.UploadTextAsync(stringfyTokenModel);
            }
        }
    }
}
