
namespace Framework.Configuration.Models
{
    /// <summary>
    /// S3ConfigurationOptions
    /// </summary>
    public class AmazonS3ConfigurationOptions
    {
        public AmazonS3ConfigurationOptions()
        {
        }

        public AmazonS3ConfigurationOptions(string region, string accessKey, string secretKey, string bucketName, string prefix, int preSignedExpiresDays)
        {
            Region = region;
            AccessKey = accessKey;
            SecretKey = secretKey;
            BucketName = bucketName;
            Prefix = prefix;
            PreSignedExpiresDays = preSignedExpiresDays;
        }

        /// <summary>
        /// Gets or sets the region to connect.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the AccessKey property for the current credentials.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the SecretKey property for the current credentials.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the bucket containing the objects.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the prefix that limits the response to keys that begin with inside bucket.
        /// </summary>
        public string Prefix { get; set; }

        public int PreSignedExpiresDays { get; set; }
    }
}
