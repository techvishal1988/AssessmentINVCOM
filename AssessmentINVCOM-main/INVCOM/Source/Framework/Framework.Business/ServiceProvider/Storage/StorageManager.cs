using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using CsvHelper.Configuration;
using EnsureThat;
using Framework.Business.Extension;
using Framework.Business.Manager;
using Framework.Configuration.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq; 
using System.Threading.Tasks;

namespace Framework.Business.ServiceProvider.Storage
{
    public class StorageManager : ManagerBase, IStorageManager<AmazonS3ConfigurationOptions>
    {
        private readonly ILogger<StorageManager> _logger;

        public StorageManager(ILogger<StorageManager> logger) : base(logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retruns list of keys filterd the response that begin with prefix in a specified bucketName.
        /// </summary>
        /// <param name="amazonS3Configuration">the amazonS3 configuration.</param>
        /// <returns> the keys.</returns>
        public async Task<IEnumerable<string>> GetFiles(AmazonS3ConfigurationOptions amazonS3Configuration)
        {
            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);

            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {
                var request = new ListObjectsV2Request()
                {
                    BucketName = amazonS3Configuration.BucketName,
                    Prefix = amazonS3Configuration.Prefix,
                };

                var listObjectsResponse = await amazonS3Client.ListObjectsV2Async(request).ConfigureAwait(false);
                var keys = listObjectsResponse.S3Objects.Select(o => o.Key).ToList();

                return keys;
            }
        }

        /// <summary>
        /// Retrive object from S3 based on key and get all records in CSV file and converts each to type T.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="amazonS3Configuration">the amazonS3 configuration.</param>
        /// <param name="key">The key.</param>
        /// <param name="csvConfiguration">configuration used for reading CSV data.</param>
        /// <returns cref = "IEnumerable{T}">retrun records converted to type T.</returns>
        public async Task<Dictionary<string, string>> ReadFile(AmazonS3ConfigurationOptions amazonS3Configuration, string fileName)
        {
            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);

            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
            {
                _logger.LogError("fileName can not be null or empty and it should have valid extention");
                return null;
            }
            string key = string.IsNullOrEmpty(amazonS3Configuration.Prefix) ? fileName : $"{amazonS3Configuration.Prefix?.TrimEnd('/')}/{fileName}";
            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {

                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = amazonS3Configuration.BucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                string urlString = await amazonS3Client.GetPreSignedURLAsync(request);
                return new Dictionary<string, string> { { key, urlString } };
            }
        }
        public async Task<Dictionary<string, string>> ReadFiles(AmazonS3ConfigurationOptions amazonS3Configuration)
        {
            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);
            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                var request = new  ListObjectsV2Request()
                {
                    BucketName = amazonS3Configuration.BucketName,
                    Prefix = amazonS3Configuration.Prefix,
                };

                var listObjectsResponse = await amazonS3Client.ListObjectsV2Async(request).ConfigureAwait(false);
                var keys = listObjectsResponse.S3Objects.Select(o => o.Key).ToList();
                foreach (var key in keys)
                {
                    var preSrequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = amazonS3Configuration.BucketName,
                        Key = amazonS3Configuration.Prefix,
                        Expires = DateTime.UtcNow.AddMinutes(amazonS3Configuration.PreSignedExpiresDays)
                    };
                    string urlString = await amazonS3Client.GetPreSignedURLAsync(preSrequest);
                    result.Add(key, urlString);
                }

                return result;
            }
        }

        /// <summary>
        /// Retrive object from S3 based on key and get all records in CSV file and converts each to type T.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="amazonS3Configuration">the amazonS3 configuration.</param>
        /// <param name="key">The key.</param>
        /// <param name="csvConfiguration">configuration used for reading CSV data.</param>
        /// <returns cref = "IEnumerable{T}">retrun records converted to type T.</returns>
        public async Task<IEnumerable<T>> ReadFileWithCsv<T>(AmazonS3ConfigurationOptions amazonS3Configuration, string key, CsvConfiguration csvConfiguration)
        {

            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);

            IEnumerable<T> result = null;

            if (string.IsNullOrWhiteSpace(key))
            {
                return result;
            }

            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {
                var request = new GetObjectRequest()
                {
                    BucketName = amazonS3Configuration.BucketName,
                    Key = key,
                };

                using (var response = await amazonS3Client.GetObjectAsync(request).ConfigureAwait(false))
                {
                    using (var reader = new StreamReader(response.ResponseStream))
                    {
                        using (var csv = new CsvReader(reader, csvConfiguration))
                        {
                            result = csv.GetRecords<T>().ToList();
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Retrive objects from S3 based on keys filterd the response that begin with prefix in a specified bucketName and get all records in CSV file and converts each to type T.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="amazonS3Configuration">the amazonS3 configuration.</param>
        /// <param name="csvConfiguration">configuration used for reading CSV data.</param>
        /// <returns cref = "IEnumerable{T}">retrun records converted to type T.</returns>
        public async Task<IEnumerable<T>> ReadFileswithCsv<T>(AmazonS3ConfigurationOptions amazonS3Configuration, CsvConfiguration csvConfiguration)
        {
            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);

            var keys = await GetFiles(amazonS3Configuration).ConfigureAwait(false);

            List<T> result = new List<T>();

            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {
                foreach (string key in keys)
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    var request = new GetObjectRequest()
                    {
                        BucketName = amazonS3Configuration.BucketName,
                        Key = key,
                    };

                    using (var response = await amazonS3Client.GetObjectAsync(request).ConfigureAwait(false))
                    {
                        using (var reader = new StreamReader(response.ResponseStream))
                        {
                            using (var csv = new CsvReader(reader, csvConfiguration))
                            {
                                result.AddRange(csv.GetRecords<T>().ToList());
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        ///  Upload file to the s3 bucket
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="amazonS3Configuration">the amazonS3 configuration.</param>
        /// <param name="fileName">fileName that to be uploaded.</param>
        ///  <param name="fileContent">file Content that to be uploaded.</param>
        /// <returns">retrun true if upload success else flase.</returns>
        public async Task<bool> UploadFileAsync(AmazonS3ConfigurationOptions amazonS3Configuration, string fileName, Stream fileContent)
        {
            EnsureArg.IsNotNull(amazonS3Configuration, nameof(amazonS3Configuration));

            var credentials = new BasicAWSCredentials(amazonS3Configuration.AccessKey, amazonS3Configuration.SecretKey);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }
            string key = string.IsNullOrEmpty(amazonS3Configuration.Prefix) ? fileName : $"{amazonS3Configuration.Prefix?.TrimEnd('/')}/{fileName}";
            using (var amazonS3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.GetBySystemName(amazonS3Configuration.Region)))
            {

                var response = await amazonS3Client.PutObjectAsync(new PutObjectRequest { BucketName = amazonS3Configuration.BucketName, Key = key, InputStream = fileContent }).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _logger.LogError($"error occoured with http status code {response.HttpStatusCode} while uploading a file", response);
                    return false;
                }
            }
        }
    }
}
