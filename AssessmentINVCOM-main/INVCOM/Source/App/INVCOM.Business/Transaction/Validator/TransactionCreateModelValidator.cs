using FluentValidation;
using INVCOM.Business.Transaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVCOM.Business.Transaction.Validator
{
    public class TransactionCreateModelValidator : AbstractValidator<TransactionCreateModel>
    {
        public TransactionCreateModelValidator()
        {
            RuleFor(x => x.TransactionAmount).GreaterThan(0).WithMessage("Transaction amount must be greater than 0").NotEmpty().WithMessage("Transaction amount is required");

            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("CustomerId must be greater than 0").NotEmpty().WithMessage("Customer Id is required");
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
            RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("Transaction Date is required");

            
        }
        
    }
}
