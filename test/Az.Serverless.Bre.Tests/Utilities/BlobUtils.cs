using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;

namespace Az.Serverless.Bre.Tests.Utilities
{
    internal static class BlobUtils
    {
        internal static BlobContainerClient MockBlobContainerClient(string? blobFilePath)
        {
            var mock = new Mock<BlobContainerClient>();

            mock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(MockBlobClient(blobFilePath));

            return mock.Object;
        }

        internal static BlobClient MockBlobClient(string? blobFilePath)
        {

            var mock = new Mock<BlobClient>();

            if (blobFilePath == null)
            {

                mock.Setup(x => x.DownloadContentAsync())
               .Throws(new Exception());

                return mock.Object;

            }

            var blobDownLoadResult = BlobsModelFactory.BlobDownloadResult(
                    content: new BinaryData(File.ReadAllBytes(blobFilePath)),
                    details: null
                );

            Response<BlobDownloadResult> response = Response.FromValue(blobDownLoadResult, Mock.Of<Response>());

            mock.Setup(x => x.DownloadContentAsync())
                .ReturnsAsync(response);

            return mock.Object;

        }
    }
}
