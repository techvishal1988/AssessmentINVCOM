namespace INVCOM.DataAccess.Repository
{
    using Framework.DataAccess.Repository;
    using INVCOM.DataAccess.Repository.Model;
    using INVCOM.Entity.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ITransctionRepository" />.
    /// </summary>
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> SearchTransactionByFilter(TransactionSearchModel searchModel);

        Task<IEnumerable<Transaction>> GetActiveTransaction();
    }
}
