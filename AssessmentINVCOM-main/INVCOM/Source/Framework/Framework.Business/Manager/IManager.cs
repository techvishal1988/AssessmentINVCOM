using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Business.Manager
{
    public interface IManager<TReadModel> : IManagerBase
    where TReadModel : class
    {
        Task<IEnumerable<TReadModel>> GetAll(CancellationToken cancellationToken);
        Task<TReadModel> GetByKey<TKey>(TKey key, CancellationToken cancellationToken);

    }
}
