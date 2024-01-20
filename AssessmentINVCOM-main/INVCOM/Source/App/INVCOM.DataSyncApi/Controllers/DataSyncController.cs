using Amazon.SQS.Model;
using AutoMapper;
using Framework.Business.ServiceProvider.Queue;
using Framework.Configuration.Models;
using INVCOM.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading;

namespace INVCOM.DataSyncApi.Controllers;

[Route("api/[controller]")]
public class DataSyncController : ControllerBase
{
    private readonly IQueueManager<AmazonSQSConfigurationOptions, List<Message>> _queueManager;

    private readonly AmazonSQSConfigurationOptions _amazonSQSConfigurationOptions;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public DataSyncController(IQueueManager<AmazonSQSConfigurationOptions, List<Message>> queueManager, ApplicationOptions applicationOptions, ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _queueManager = queueManager;
        _amazonSQSConfigurationOptions = applicationOptions.amazonSQSConfigurationOptions;
        _mapper = mapper;
    }


    // POST api/values
    [HttpPost]
    public async Task Post([FromBody] Guid referenceNumber, CancellationToken cancellationToken)
    {

        if (referenceNumber == Guid.Empty)
        {
            var dbEntitys = await _transactionRepository.GetActiveTransaction().ConfigureAwait(false);
            if (dbEntitys is not null)
            {
                foreach (var entity in dbEntitys)
                {
                    var dbTransaction = _mapper.Map<Entity.Entities.Transaction>(entity);
                    await _queueManager.SendMessageAsync(_amazonSQSConfigurationOptions, JsonConvert.SerializeObject(entity.ReferenceNumber), cancellationToken);

                    dbTransaction.IsSynced = true;
                    var updatedTransaction = await _transactionRepository.UpdateAsync(dbTransaction, default).ConfigureAwait(false);
                }
            }
        }
        else
        {
            var dbEntity = await _transactionRepository.GetByKey(referenceNumber, default).ConfigureAwait(false);
            if (dbEntity is not null)
            {
                var dbTransaction = _mapper.Map<Entity.Entities.Transaction>(dbEntity);

                await _queueManager.SendMessageAsync(_amazonSQSConfigurationOptions, JsonConvert.SerializeObject(referenceNumber), cancellationToken);

                dbTransaction.IsSynced = true;
                var updatedTransaction = await _transactionRepository.UpdateAsync(dbTransaction, default).ConfigureAwait(false);
            }
        }
    }
}