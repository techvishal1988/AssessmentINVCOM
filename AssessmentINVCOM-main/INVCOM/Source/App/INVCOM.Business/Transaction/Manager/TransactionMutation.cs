using AutoMapper;
using FluentValidation.Results;
using GraphQL;
using GraphQL.Types;
using INVCOM.Business.GraphQL;
using INVCOM.Business.GraphQL.Model;
using INVCOM.Business.Transaction.Models;
using INVCOM.Business.Transaction.Types;
using INVCOM.Business.Transaction.Validator;
using INVCOM.DataAccess.Repository;
using System;
using System.Threading.Tasks;

namespace INVCOM.Business.Transaction.Manager
{
    public class TransactionMutation : ITopLevelMutation
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly TransactionCreateModelValidator _transactionCreateValidator;
        private readonly TransactionUpdateModelValidator _transactionUpdateValidator;
        private readonly IMapper _mapper;
        public TransactionMutation(ITransactionRepository transactionRepository, TransactionCreateModelValidator transactionCreateValidator, TransactionUpdateModelValidator transactionUpdateValidator, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _transactionCreateValidator = transactionCreateValidator;
            _transactionUpdateValidator = transactionUpdateValidator;
            _mapper = mapper;
        }
        public void RegisterField(ObjectGraphType graphType)
        {
            graphType.Field<TransactionType>("createTransaction")
               .Argument<NonNullGraphType<TransactionCreateInputType>>("ctrasction", "object of ctrasction")
               .ResolveAsync(async context => await ResolveCreateTransaction(context).ConfigureAwait(false));

            graphType.Field<TransactionType>("updateTransaction")
                .Argument<NonNullGraphType<TransactionUpdateInputType>>("ctrasction", "object of ctrasction")
                .ResolveAsync(async context => await ResolveUpdateTransaction(context).ConfigureAwait(false));

            graphType.Field<StringGraphType>("deleteTransaction")
            .Argument<NonNullGraphType<IdGraphType>>("ctrasctionId", "id of ctrasction")
            .ResolveAsync(async context => await ResolveDeleteTransaction(context).ConfigureAwait(false));

            
        }

        private async Task<TransactionReadModel> ResolveCreateTransaction(IResolveFieldContext<object> context)
        {
            var ctrasction = context.GetArgument<TransactionCreateModel>("ctrasction");

            var validationResult = _transactionCreateValidator.Validate(ctrasction);

            if (!validationResult.IsValid)
            {
                LoadErrors(context, validationResult);
                return null;
            }

            //if (dbData != null)
            //{
            //    context.Errors.Add(new ExecutionError("Transaction already exists"));
            //    return null;
            //}

            var dbEntity = _mapper.Map<Entity.Entities.Transaction>(ctrasction);
            dbEntity.Id = Guid.NewGuid();
            var addedTransaction = await _transactionRepository.CreateAsync(dbEntity, default).ConfigureAwait(false);
            var result = _mapper.Map<TransactionReadModel>(addedTransaction);
            return result;

        }
        

        private async Task<TransactionReadModel> ResolveUpdateTransaction(IResolveFieldContext<object> context)
        {

            var ctrasctionUpdateModel = context.GetArgument<TransactionUpdateModel>("ctrasction");

            var validationResult = _transactionUpdateValidator.Validate(ctrasctionUpdateModel);

            if (!validationResult.IsValid)
            {
                LoadErrors(context, validationResult);
                return null;
            }

            var dbEntity = await _transactionRepository.GetByKey(ctrasctionUpdateModel.Id, default).ConfigureAwait(false);
            if (dbEntity == null)
            {
                context.Errors.Add(new ExecutionError("Couldn't find ctrasction in db."));
                return null;
            }
            var dbTransaction = _mapper.Map<Entity.Entities.Transaction>(ctrasctionUpdateModel);
            var updatedTransaction = await _transactionRepository.UpdateAsync(dbTransaction, default).ConfigureAwait(false);
            return _mapper.Map<TransactionReadModel>(updatedTransaction);

        }

        private async Task<object> ResolveDeleteTransaction(IResolveFieldContext<object> context)
        {

            var ctrasctionId = context.GetArgument<Guid>("ctrasctionId");

            var dbEntity = await _transactionRepository.GetByKey(ctrasctionId, default).ConfigureAwait(false);

            if (dbEntity == null)
            {
                context.Errors.Add(new ExecutionError("Couldn't find ctrasction in db."));
                return null;
            }

            await _transactionRepository.DeleteAsync(ctrasctionId, default).ConfigureAwait(false);

            var res = new MutationResponse()
            {
                Message = $"The ctrasction with the id: {ctrasctionId} has been successfully deleted from db."
            };
            return res.Message;
        }

        private static void LoadErrors(IResolveFieldContext<object> context, ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                context.Errors.Add(new ExecutionError(error.ErrorMessage));
            }
        }
    }
}
