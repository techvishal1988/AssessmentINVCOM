using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Configuration.Models
{
    /// <summary>
    /// AmazonSQSConfigurationOptions
    /// </summary>
    public class AmazonSQSConfigurationOptions
    {
        public AmazonSQSConfigurationOptions()
        {
        }

        public AmazonSQSConfigurationOptions(string region, string accessKey, string secretKey, string sqsQueueName, int maxNumberOfMessages, int waitTimeInseconds)
        {
            Region = region;
            AccessKey = accessKey;
            SecretKey = secretKey;
            SQSQueueUrl = sqsQueueName;
            MaxNumberOfMessages = maxNumberOfMessages;
            WaitTimeInseconds = waitTimeInseconds;
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
        /// Gets or sets the name of the SQS Queue.
        /// </summary> 
        public string SQSQueueUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the MaxNumberOfMessages
        /// </summary> 
        public int MaxNumberOfMessages { get; set; }

        /// <summary>
        /// Gets or sets the name of the WaitTimeInseconds
        /// </summary> 
        public int WaitTimeInseconds { get; set; }
    }
}
