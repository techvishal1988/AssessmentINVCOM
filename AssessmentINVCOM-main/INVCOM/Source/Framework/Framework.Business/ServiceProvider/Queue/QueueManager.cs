namespace Framework.Business.ServiceProvider.Queue
{
    using Amazon.Runtime;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using EnsureThat;
    using Framework.Business.Manager;
    using Framework.Configuration.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueueManager : ManagerBase, IQueueManager<AmazonSQSConfigurationOptions, List<Message>>
    {
        private readonly ILogger<QueueManager> _logger;

        public QueueManager(ILogger<QueueManager> logger) : base(logger)
        {
            _logger = logger;
        }

        public async Task Initialize(AmazonSQSConfigurationOptions config)
        {
            EnsureArg.IsNotNull(config, nameof(config));
            var credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            using (var amazonSQSClient = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.GetBySystemName(config.Region)))
            {
                try
                {
                    CreateQueueRequest request = new CreateQueueRequest()
                    {
                        QueueName = new System.Uri(config.SQSQueueUrl).Segments.Last()
                    };
                    await amazonSQSClient.CreateQueueAsync(request);
                    _logger.LogInformation($"Queue Created successfully {config.SQSQueueUrl}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error while creating queue");
                    throw;
                }
            }
        }
        /// <summary>
        /// SendMessageAsync responsible to push message to the queue
        /// </summary>
        /// <param name="config">Queue config</param>
        /// <param name="Content">Queue Message Content</param>
        /// <returns></returns>
        public async Task<bool> SendMessageAsync(AmazonSQSConfigurationOptions config, string Content, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(config, nameof(config));
            var credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            using (var amazonSQSClient = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.GetBySystemName(config.Region)))
            {
                SendMessageRequest sendMessageRequest = new SendMessageRequest
                {
                    QueueUrl = config.SQSQueueUrl,
                    MessageBody = Content
                };
                SendMessageResponse response = await amazonSQSClient.SendMessageAsync(sendMessageRequest, cancellationToken).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _logger.LogError($"error occoured with http status code {response.HttpStatusCode} while sending the message to the queue {config.SQSQueueUrl}", response);
                    return false;
                }
            }
        }
        /// <summary>
        /// ReceiveMessageAsync responsible to receive message from the queue
        /// </summary>
        /// <param name="config">Queue config</param>
        /// <returns></returns>
        public async Task<List<Message>> ReceiveMessageAsync(AmazonSQSConfigurationOptions config, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(config, nameof(config));
            var credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            using (var amazonSQSClient = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.GetBySystemName(config.Region)))
            {
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest()
                {
                    QueueUrl = config.SQSQueueUrl,
                    MaxNumberOfMessages = config.MaxNumberOfMessages,
                    WaitTimeSeconds = config.WaitTimeInseconds
                };
                ReceiveMessageResponse response = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Messages.Any() ? response.Messages : new List<Message>();
                }
                else
                {
                    _logger.LogError($"error occoured with http status code {response.HttpStatusCode} while receiving the message from the queue {config.SQSQueueUrl}", response);
                    return null;
                }
            }
        }

        /// <summary>
        /// DeleteMessageAsync responsible to delete message from the queue
        /// </summary>
        /// <param name="config">Queue config</param>
        /// <returns></returns>
        public async Task<bool> DeleteMessageAsync(AmazonSQSConfigurationOptions config, string messageReceiptHandle, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(config, nameof(config));

            var credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            using (var amazonSQSClient = new AmazonSQSClient(credentials, Amazon.RegionEndpoint.GetBySystemName(config.Region)))
            {
                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = config.SQSQueueUrl,
                    ReceiptHandle = messageReceiptHandle
                };

                var response = await amazonSQSClient.DeleteMessageAsync(deleteRequest, cancellationToken).ConfigureAwait(false);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    _logger.LogError($"error occoured with http status code {response.HttpStatusCode} while deleting the message from the queue {config.SQSQueueUrl}", response);
                    return false;
                }
            }
        }
    }
}

