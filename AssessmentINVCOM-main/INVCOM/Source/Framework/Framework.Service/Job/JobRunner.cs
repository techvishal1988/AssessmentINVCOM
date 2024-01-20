namespace Framework.Service.Job
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class JobRunner
    {
        public const string RetrySeconds = "RetrySeconds";  // RetrySeconds PropertyName in JobResponse
        public const string JobKey = "JobKey";  // JobKey Propertyname in JobResponse
        public const string JobStatusPropertyName = "JobStatus";  // JobStatus PropertyName in JobResponse. Name conflict with JobStatus.cs so can not name it to JobStatus as the other two constants

        /// <summary>
        /// Runs the job asynchronous. Assume using Domain Client (for example: ReportingDataClient.cs) code to call the job end point.
        /// </summary>
        /// <typeparam name="T1">The return type of runJobFunc.</typeparam>
        /// <typeparam name="T2">The return type of checkjobStatusFunc.</typeparam>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="runJobFunc">The run job function.</param>
        /// <param name="checkJobStatusFunc">The check job status function.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>Task.<T2></T2></returns>
        public static async Task<T2> RunJobAsync<T1, T2>(string jobName, Func<Task<T1>> runJobFunc, Func<Guid?, Task<T2>> checkJobStatusFunc, ILogger logger)
            where T1 : class
            where T2 : class
        {
            EnsureArg.IsNotNull(runJobFunc, nameof(runJobFunc));
            EnsureArg.IsNotNull(checkJobStatusFunc, nameof(checkJobStatusFunc));

            var runJobResult = await runJobFunc().ConfigureAwait(false);
            var responseValues = GetPropertyValues(runJobResult, new List<string>() { RetrySeconds, JobKey });

            var jobKey = new Guid(responseValues[JobKey].ToString());
            var retrySeconds = responseValues[RetrySeconds] == null ? 5 : Convert.ToInt32(responseValues[RetrySeconds], NumberFormatInfo.InvariantInfo);

            var checkStatusResult = await checkJobStatusFunc(jobKey).ConfigureAwait(false);
            var checkStatusValues = GetPropertyValues(checkStatusResult, new List<string>() { JobStatusPropertyName });

            while (checkStatusValues[JobStatusPropertyName].ToString() == JobStatus.Running.ToString())
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(retrySeconds)).ConfigureAwait(false);
                    checkStatusResult = await checkJobStatusFunc(jobKey).ConfigureAwait(false);
                    checkStatusValues = GetPropertyValues(checkStatusResult, new List<string>() { JobStatusPropertyName });
                    logger.LogInformation($"{jobName} - finish check status, status is: {checkStatusValues[JobStatusPropertyName].ToString()}");
                });

                try
                {
                    task.Wait();
                }
                catch (Exception exception)
                {
                    var errorMessage = await GetJobRunnerErrorMessages(exception).ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        logger.LogCritical($"Error occurred in {nameof(RunJobAsync)} for jobName: [{jobName}] {errorMessage}");
                        throw new Exception($"Running job [{jobName}] failed with exception: {errorMessage}");
                    }
                    else
                    {
                        logger.LogCritical(exception, $"Error occurred in {nameof(RunJobAsync)} for jobName: [{jobName}]");
                    }

                    throw new Exception($"Running job [{jobName}] failed with exception", exception);
                }
            }

            return checkStatusResult;
        }

        private static async Task<string> GetJobRunnerErrorMessages(Exception exception)
        {
            EnsureArg.IsNotNull(exception, nameof(Exception));
            var result = string.Empty;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                dynamic ex = exception;
                var jobErrorResponse = ex.GetType().GetProperty("Result") != null ? ex.Result : null;
                if (jobErrorResponse != null && jobErrorResponse.HasError)
                {
                    result = JsonConvert.SerializeObject(jobErrorResponse.ErrorRecords, Formatting.Indented);
                }
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        private static Dictionary<string, object> GetPropertyValues(object obj, List<string> propertyNames)
        {
            var dictionary = new Dictionary<string, object>();

            Type t = obj.GetType();

            foreach (var propertyName in propertyNames)
            {
                PropertyInfo prop = t.GetProperty(propertyName);
                dictionary.Add(propertyName, prop.GetValue(obj));
            }

            return dictionary;
        }
    }
}
