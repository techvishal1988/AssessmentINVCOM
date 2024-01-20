namespace Framework.DataAccess.Repository
{
    using Framework.Configuration.Models;
    using Framework.DataAccess.Model;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using static Org.BouncyCastle.Math.EC.ECCurve;

    public interface IGenericRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken);

        Task<TEntity> GetByKey<TKey>(TKey Id, CancellationToken cancellationToken);

        Task<TEntity> GetByKey<TKey, TRangeKey>(TKey key, TRangeKey rangeKey, CancellationToken cancellationToken);

        Task<PagedResultModel<TEntity>> GetPaginatedScanItemsAsync(int pageSize = 5, string lastEvaluatedKey = null, IEnumerable<string> projectionFields = default);

        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync<TKey>(TKey key, CancellationToken cancellationToken);

        Task<string> UploadFileAsync(AmazonS3ConfigurationOptions config, string fileName, Stream fileContent);

    }
}
