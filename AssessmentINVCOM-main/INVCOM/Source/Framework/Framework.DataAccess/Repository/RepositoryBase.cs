namespace Framework.DataAccess.Repository
{    
    using Amazon.DynamoDBv2.DataModel;    
    public abstract class RepositoryBase<TEntity> : IRepositoryBase
    {
        protected readonly IDynamoDBContext _dynamoDBContext;
        public RepositoryBase(IDynamoDBContext context)
        {
            _dynamoDBContext = context;
        }
    }    
}