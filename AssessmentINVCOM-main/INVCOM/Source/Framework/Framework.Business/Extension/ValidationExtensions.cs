namespace Framework.Business.Extension
{
    using EnsureThat;    
    using FluentValidation.Results;
    using Framework.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="ValidationExtensions" />.
    /// </summary>
    public static class ValidationExtensions
    { 

        /// <summary>
        /// The WithErrorEnum.
        /// </summary>
        /// <param name="validationFailure">The validationFailure<see cref="ValidationFailure"/>.</param>
        /// <param name="errorCode">The errorCode<see cref="Enum"/>.</param>
        /// <returns>The <see cref="ValidationFailure"/>.</returns>
        public static ValidationFailure WithErrorEnum(this ValidationFailure validationFailure, Enum errorCode)
        {
            validationFailure.ErrorCode = errorCode.ToString();

            return validationFailure;
        }

        /// <summary>
        /// The ToErrorMessages.
        /// </summary>
        /// <typeparam name="TErrorCode">.</typeparam>
        /// <param name="validationResult">The validationResult<see cref="ValidationResult"/>.</param>
        /// <returns>The <see cref="ErrorMessages{TErrorCode}"/>.</returns>
        public static ErrorMessages<TErrorCode> ToErrorMessages<TErrorCode>(this ValidationResult validationResult)
            where TErrorCode : struct, Enum
        {
            var errorMessages = new ErrorMessages<TErrorCode>();

            foreach (var error in validationResult.Errors)
            {
                errorMessages.Add(new ErrorMessage<TErrorCode>(error));
            }

            return errorMessages;
        }




        /// <summary>
        /// Merges the first set with the second set.
        /// DO NOT call merge more than once. Use Concat to append multiple sets prior to a merge.
        /// </summary>
        /// <typeparam name="TErrorCode">The type of the error code.</typeparam>
        /// <param name="first">The first set of errorRecords.</param>
        /// <param name="second">The second set of errorRecords.</param>
        /// <returns>ErrorRecords{TErrorCode}.</returns>
        public static ErrorRecords<TErrorCode> Merge<TErrorCode>(this IEnumerable<ErrorRecord<TErrorCode>> first, IEnumerable<ErrorRecord<TErrorCode>> second)
            where TErrorCode : struct, Enum
        {
            EnsureArg.IsNotNull(first, nameof(first));
            EnsureArg.IsNotNull(second, nameof(second));

            return new ErrorRecords<TErrorCode>(first.Concat(second).GroupBy(x => x.OrdinalPosition)
                .Select(x => new ErrorRecord<TErrorCode>(x.First().Id, x.Key, x.SelectMany(record => record.Errors)))
                .OrderBy(x => x.OrdinalPosition));
        }





        /// <summary>
        /// The ToFormattedString.
        /// </summary>
        /// <typeparam name="TErrorCode">.</typeparam>
        /// <param name="errorRecords">The errorRecords<see cref="IEnumerable{ErrorRecord{TErrorCode}}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string ToFormattedString<TErrorCode>(this IEnumerable<ErrorRecord<TErrorCode>> errorRecords)
            where TErrorCode : struct, Enum
        {
            EnsureArg.IsNotNull(errorRecords, nameof(errorRecords));

            var stringBuilder = new StringBuilder();

            foreach (var errorRecord in errorRecords)
            {
                stringBuilder.AppendLine(errorRecord.ToFormattedString());
            }

            return stringBuilder.ToString();
        }
    }
}
