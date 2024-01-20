namespace Framework.Business.ServiceProvider.Queue
{
    using Framework.Business.Manager;
    using Framework.Configuration.Models;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueueManager<TIn, TOut> : IManagerBase where TIn : class where TOut : class
    {
        public Task<bool> SendMessageAsync(TIn amazonSQSConfiguration, string MessageContent, CancellationToken cancellationToken);
        public Task<TOut> ReceiveMessageAsync(TIn amazonSQSConfiguration, CancellationToken cancellationToken);
        public Task<bool> DeleteMessageAsync(TIn amazonSQSConfiguration, string messageReceiptHandle, CancellationToken cancellationToken);

        Task Initialize(TIn amazonSQSConfiguration);
    }
}
