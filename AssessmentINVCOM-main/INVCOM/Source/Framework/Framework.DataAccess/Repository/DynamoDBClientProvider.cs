namespace Framework.DataAccess.Repository
{
    using System;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;    
    using Microsoft.Extensions.Logging;
    public abstract class DynamoDBClientProviderBase
    {
        private readonly ILogger<DynamoDBClientProviderBase> _logger;
        private readonly IAmazonDynamoDB _client;

        public DynamoDBClientProviderBase(ILogger<DynamoDBClientProviderBase> logger, IAmazonDynamoDB amazonDynamoDBClient)
        {
            _logger = logger;
            _client = amazonDynamoDBClient;
        }
        public abstract Task CreateTable();
        public async Task Initialize(string tableName)
        {
            await CreateTable(tableName);
        }
        public async Task CreateTable(string tableName)
        {
            try
            {
                bool tableExist = await isTableExistAsync(tableName);
                if (!tableExist)
                {
                    await CreateTable();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public async Task<bool> isTableExistAsync(string tableName)
        {
            try
            {
                var result = await _client.DescribeTableAsync(tableName);
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            return true;
        }
    }
}
