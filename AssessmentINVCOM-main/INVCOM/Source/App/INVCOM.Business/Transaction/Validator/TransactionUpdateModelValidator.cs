using FluentValidation;
using INVCOM.Business.Transaction.Models;

namespace INVCOM.Business.Transaction.Validator
{
    public class TransactionUpdateModelValidator : AbstractValidator<TransactionUpdateModel>
    {
        public TransactionUpdateModelValidator()
        {
            RuleFor(x => x.ReferenceNumber).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.TransactionAmount).GreaterThan(0).WithMessage("Transaction amount must be greater than 0").NotEmpty().WithMessage("Transaction amount is required");

            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("CustomerId must be greater than 0").NotEmpty().WithMessage("Customer Id is required");
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
            RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("Transaction Date is required");
            RuleFor(x => x.TransactionUpdatedDate).NotEmpty().WithMessage("Transaction Updated Date is required");
            RuleFor(x => x.ReferenceNumber).NotEmpty().WithMessage("Reference Number is required");
        }

    }
}
