using Az.Serverless.Bre.Func01.Repositories.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Repositories.Implementations
{
    public class BlobRulesStoreRepository : IRulesStoreRepository
    {
        private readonly BlobContainerClient _blobContainerClient;
        public BlobRulesStoreRepository(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient
                ?? throw new ArgumentNullException(nameof(blobContainerClient));

        }

        public async Task<object> GetConfigAsync(string configFileName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(configFileName);

            try
            {
                BlobDownloadResult blobDownloadResult = await blobClient.DownloadContentAsync()
                        .ConfigureAwait(false);

                return JsonConvert.DeserializeObject(blobDownloadResult.Content.ToString());
            }
            catch
            {

                return null;
            }


        }
    }
}
