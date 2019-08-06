using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Postcore.Web.Core.Interfaces;

namespace Postcore.Web.Infrastructure.Utilities
{
    public class S3FileUploader : IFileUploader
    {
        private readonly IConfiguration _configuration;

        public S3FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream storageStream, string bucket)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name must be specified.");

            using (var client = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                if (storageStream.Length > 0)
                    if (storageStream.CanSeek)
                        storageStream.Seek(0, SeekOrigin.Begin);

                var request = new PutObjectRequest
                {
                    AutoCloseStream = true,
                    BucketName = bucket,
                    InputStream = storageStream,
                    Key = fileName
                };
                var response = await client.PutObjectAsync(request).ConfigureAwait(false);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
        }
    }
}
