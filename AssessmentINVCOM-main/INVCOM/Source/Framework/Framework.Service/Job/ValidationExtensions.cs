namespace Framework.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EnsureThat;
    using FluentValidation;
    using FluentValidation.Results;
    using Framework.Service;

    public static class ValidationExtensions
    {
        

        public static ErrorMessages<TErrorCode> ToErrorMessages<TErrorCode>(this ValidationResult validationResult)
            where TErrorCode : struct, Enum
        {
            EnsureArg.IsNotNull(validationResult, nameof(validationResult));

            var errorMessages = new ErrorMessages<TErrorCode>();

            foreach (var error in validationResult.Errors)
            {
                errorMessages.Add(new ErrorMessage<TErrorCode>(error));
            }

            return errorMessages;
        }

       
    }
}
