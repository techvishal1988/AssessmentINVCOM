using AutoMapper;
using Framework.DataAccess.Repository;
using Framework.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Business.Manager
{
    public abstract class Manager<TEntity,TReadModel> : BaseManager<TEntity>, IManager<TReadModel>
       where TEntity : BaseEntity
       where TReadModel : class, new()
    {
        protected Manager(IGenericRepository<TEntity> genericRepository, ILogger<Manager<TEntity,  TReadModel>> logger, IMapper mapper)
            : base(genericRepository, logger, mapper)
        {
        }

        /// <summary>
        /// The GetItemAsync.
        /// </summary>
        /// <param name="key">The id<see cref="TKey"/>.</param>        
        /// <returns>The <see cref="Task{IEnumerable{TReadModel}}"/>.</returns>
        public virtual async Task<TReadModel> GetByKey<TKey>(TKey key, CancellationToken cancellationToken)
        {
            var readModels = new TReadModel();
            var data = await Repository.GetByKey(key, cancellationToken);
            var models = Mapper.Map(data, readModels);
            await QueryAfterMapAsync(new[] { models }, new[] { data }).ConfigureAwait(false);
            return models;
        }

        /// <summary>
        /// The GSI1QueryAllAsync.
        /// </summary>
        /// <returns>The <see cref="Task{IEnumerable{TReadModel}}"/>.</returns>
        public virtual async Task<IEnumerable<TReadModel>> GetAll(CancellationToken cancellationToken)
        {
            var readModels = new List<TReadModel>();
            var data = await Repository.GetAll(cancellationToken);
            var models = Mapper.Map(data, readModels);
            await QueryAfterMapAsync(models, data).ConfigureAwait(false);
            return models;
        }

        /// <summary>
        /// The QueryAfterMapAsync.
        /// </summary>
        /// <param name="models">The models<see cref="IEnumerable{TReadModel}"/>.</param>
        /// <param name="entities">The entities<see cref="IEnumerable{TEntity}"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        protected virtual Task QueryAfterMapAsync(IEnumerable<TReadModel> models, IEnumerable<TEntity> entities)
        {
            return Task.CompletedTask;
        }
    }
}
