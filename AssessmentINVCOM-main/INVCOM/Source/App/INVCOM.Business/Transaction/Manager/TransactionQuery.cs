using AutoMapper;
using GraphQL;
using GraphQL.Types;
using INVCOM.Business.GraphQL;
using INVCOM.Business.Transaction.Models;
using INVCOM.Business.Transaction.Types;
using INVCOM.DataAccess.Repository;
using INVCOM.DataAccess.Repository.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace INVCOM.Business.Transaction.Manager
{
    public class TransactionQuery : ITopLevelQuery
    {
        private readonly ITransactionRepository _transcationRepository;
        private readonly IMapper _mapper;
        public TransactionQuery(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transcationRepository = transactionRepository;
            _mapper = mapper;
        }
        public void RegisterField(ObjectGraphType graphType)
        {
            graphType.Field<ListGraphType<TransactionType>>("Transactions")
            .ResolveAsync(async context => await ResolveTransactions().ConfigureAwait(false));

            graphType.Field<TransactionType>("GetByReferenceNumber")
            .Argument<NonNullGraphType<IdGraphType>>("ReferenceNumber", "id of the transaction")
            .ResolveAsync(async context => await ResolveTransaction(context).ConfigureAwait(false));

            graphType.Field<ListGraphType<TransactionType>>("searchTransaction")
            .Argument<TransactionSearchInputType>("searchInput", "object of the search input")
            .ResolveAsync(async context => await ResolveSearchTransaction(context).ConfigureAwait(false));
        }

        private async Task<IEnumerable<TransactionReadModel>> ResolveTransactions()
        {
            var dbTransaction = await _transcationRepository.GetAll(default).ConfigureAwait(false);
            var res = _mapper.Map<IEnumerable<TransactionReadModel>>(dbTransaction);
            return res;
        }

        private async Task<TransactionReadModel> ResolveTransaction(IResolveFieldContext<object> context)
        {
            var key = context.GetArgument<Guid>("ReferenceNumber");
            if (key != Guid.Empty)
            {
                var dbTransaction = await _transcationRepository.GetByKey(key, default).ConfigureAwait(false);
                return _mapper.Map<TransactionReadModel>(dbTransaction);
            }
            else
            {
                context.Errors.Add(new ExecutionError("Wrong value for id"));
                return null;
            }
        }

        private async Task<IEnumerable<TransactionReadModel>> ResolveSearchTransaction(IResolveFieldContext<object> context)
        {
            var searchModel = context.GetArgument<TransactionSearchModel>("searchInput");
            if (searchModel !=  null)
            {
                var result  = await _transcationRepository.SearchTransactionByFilter(searchModel).ConfigureAwait(false);
                return _mapper.Map<IEnumerable<TransactionReadModel>>(result);
            }
            else
            {
                context.Errors.Add(new ExecutionError("Wrong value for search input"));
                return null;
            }
        }

    }
}
