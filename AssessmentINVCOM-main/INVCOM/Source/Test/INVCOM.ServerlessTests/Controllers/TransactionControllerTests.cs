using INVCOM.Business.Transaction.Models;
using INVCOM.ServerlessTests;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Assert = Xunit.Assert;

namespace INVCOM.RESTService.Controllers.Tests
{

    public class TransactionControllerTests : IClassFixture<TransactionTestFixture>
    {
        private TransactionController _transactioncontroller;
        public TransactionControllerTests(TransactionTestFixture testFixture)
        {

            _transactioncontroller = testFixture._transactionController;
        }
        [Fact]
        public async Task CreateTransactionTest()
        {
            TransactionCreateModel model = new TransactionCreateModel
            {
                CustomerId = 100,
                CustomerName = "test",
                TransactionAmount = 1000,
                TransactionDate = DateTime.Now,
                TransactionType = "Deposit",
                IsSynced = true,
                IsActive = true,
                
            };

            var contollerResult = await _transactioncontroller.CreateTransaction(model).ConfigureAwait(false);
            Assert.NotNull(contollerResult);
            Assert.Equal(200,((ObjectResult)contollerResult).StatusCode);
            var val = ((ObjectResult)contollerResult).Value;
            Assert.NotNull(val);
            Assert.NotNull(((TransactionReadModel)val).ReferenceNumber);

        }

        [Fact]
        public async Task UpdateTransactionTest()
        {
            TransactionCreateModel model = new TransactionCreateModel
            {
                CustomerId = 200,
                CustomerName = "test",
                TransactionAmount = 2000,
                TransactionDate = DateTime.Now,
                TransactionType = "Deposit",
                IsSynced = true,
                IsActive = true,

            };

            var contollerResult = await _transactioncontroller.CreateTransaction(model).ConfigureAwait(false);
            Assert.NotNull(contollerResult);
            Assert.Equal(((ObjectResult)contollerResult).StatusCode, 200);
            var val = ((ObjectResult)contollerResult).Value;
            Assert.NotNull(val);
            TransactionReadModel transactionReadModel = (TransactionReadModel)val;
            Assert.NotNull(transactionReadModel.ReferenceNumber);
            TransactionUpdateModel updateModel = new TransactionUpdateModel
            {
               
                ReferenceNumber = transactionReadModel.ReferenceNumber,
                CustomerId = transactionReadModel.CustomerId,
                CustomerName = "Updated test" ,
                TransactionAmount = 2000,
                TransactionDate = DateTime.Now, 
                TransactionUpdatedDate = DateTime.Now,
                TransactionType = "withdraw",
                IsSynced = true,
                IsActive = true,
            };

            var updateTransactionResult = await _transactioncontroller.UpdateTransaction(updateModel).ConfigureAwait(false);

            Assert.NotNull(updateTransactionResult);
            Assert.Equal(200,((ObjectResult)updateTransactionResult).StatusCode);
            var updateVal = ((ObjectResult)updateTransactionResult).Value;
            Assert.NotNull(updateVal);
            var updateTransactionReadModel = (TransactionReadModel)updateVal;

            Assert.Equal(updateModel.CustomerId, updateTransactionReadModel.CustomerId);
            Assert.Equal(updateModel.ReferenceNumber, updateTransactionReadModel.ReferenceNumber);
            Assert.Equal(updateModel.TransactionAmount, updateTransactionReadModel.TransactionAmount); 
            Assert.Equal(updateModel.TransactionType, updateTransactionReadModel.TransactionType);
            Assert.Equal(updateModel.CustomerName, updateTransactionReadModel.CustomerName); 
        }
    }
}