using AutoMapper;
using Framework.Service;
using INVCOM.Business.Transaction.Models;
using INVCOM.Business.Transaction.Validator;
using INVCOM.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

namespace INVCOM.RESTService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces(SupportedContentTypes.Json, SupportedContentTypes.Xml)]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly TransactionCreateModelValidator _transactionCreateValidator;
        private readonly TransactionUpdateModelValidator _transactionUpdateValidator;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionRepository transactionRepository, TransactionCreateModelValidator transactionCreateValidator, TransactionUpdateModelValidator transactionUpdateValidator, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _transactionCreateValidator = transactionCreateValidator;
            _transactionUpdateValidator = transactionUpdateValidator;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateModel model)
        {
            var validationResult = _transactionCreateValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }


            var dbEntity = _mapper.Map<Entity.Entities.Transaction>(model);
            dbEntity.ReferenceNumber = Guid.NewGuid();
            var addedTransaction = await _transactionRepository.CreateAsync(dbEntity, default).ConfigureAwait(false);
            var result = _mapper.Map<TransactionReadModel>(addedTransaction);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] TransactionUpdateModel model)
        {

            var validationResult = _transactionUpdateValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var dbEntity = await _transactionRepository.GetByKey(model.ReferenceNumber, default).ConfigureAwait(false);
            if (dbEntity == null)
            {
                return BadRequest("Couldn't find transaction in db.");
            }
            var dbTransaction = _mapper.Map<Entity.Entities.Transaction>(model);
            var updatedTransaction = await _transactionRepository.UpdateAsync(dbTransaction, default).ConfigureAwait(false);
            var result = _mapper.Map<TransactionReadModel>(updatedTransaction);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteTransaction([FromQuery] Guid referenceNumber)
        {
            var dbEntity = await _transactionRepository.GetByKey(referenceNumber, default).ConfigureAwait(false);
            if (dbEntity == null)
            {
                return BadRequest("Couldn't find transaction in db.");
            }
            var dbTransaction = _mapper.Map<Entity.Entities.Transaction>(dbEntity);
            dbTransaction.IsActive = false;
            var updatedTransaction = await _transactionRepository.UpdateAsync(dbTransaction, default).ConfigureAwait(false);            
            return Ok();
        }

    }
}
