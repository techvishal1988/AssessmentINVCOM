namespace Framework.Service.Job
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoData.Framework.Service;
    using EnsureThat;
    using global::Framework.Business;

    using global::Framework.Service;
    using global::Framework.Service.Job;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [JobRoute]
    [ApiController]
    public abstract class JobControllerBase : ControllerBase
    {
        private const int ExpirationFactor = 100;

        // State dictionary for jobs running on the working thread
        private static readonly ConcurrentDictionary<Guid, IJobStatus> _runningJobs = new ConcurrentDictionary<Guid, IJobStatus>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected JobControllerBase(IServiceScopeFactory serviceScopeFactory)
        {
            EnsureArg.IsNotNull(serviceScopeFactory, nameof(serviceScopeFactory));

            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Method to check the status of the job.  This is where the location header redirects to.
        /// </summary>
        /// <param name="jobKey">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpGet(nameof(CheckStatus))]
        [ProducesResponseType(typeof(JobResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JobResponse<>), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(JobResponse<>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public ObjectResult CheckStatus([FromQuery] Guid jobKey)
        {
            // If the job is complete
            if (_runningJobs.ContainsKey(jobKey))
            {
                var runningJob = _runningJobs[jobKey];
                SlideExpiration(runningJob);
                PurgeExpiredJobs();

                if (runningJob.JobStatus == JobStatus.Completed)
                {
                    AppendHeaders(runningJob.JobKey, runningJob.RetrySeconds);
                    return ToStatusCode(runningJob, successCode: HttpStatusCode.OK);
                }
                else
                {
                    AppendHeaders(runningJob.JobKey, runningJob.RetrySeconds);
                    return ToStatusCode(runningJob, successCode: HttpStatusCode.Accepted);
                }
            }
            else
            {
                PurgeExpiredJobs();
                var emptyJob = new JobResponse<JobStatus>(jobKey, JobStatus.NotFound, null, null, 0);
                //emptyJob.ErrorRecords.Add(new ErrorRecord<JobStatus>(JobStatus.NotFound, $"The job with key [{jobKey}] was not found"));
                AppendHeaders(emptyJob.JobKey, emptyJob.RetrySeconds);
                return ToStatusCode(emptyJob, errorCode: HttpStatusCode.Conflict);
            }
        }

        protected ObjectResult StartJob<TErrorCode>(Func<IServiceScope, Task<ManagerResponse<TErrorCode>>> job, int retryAfter = 30)
            where TErrorCode : struct, Enum
        {
            var jobKey = Guid.NewGuid();
            var now = DateTimeOffset.Now;
            var jobExpirationDateTime = now.AddSeconds(ExpirationFactor * retryAfter);
            var runningJob = new JobResponse<TErrorCode>(jobKey, JobStatus.Running, DateTimeOffset.Now, jobExpirationDateTime, retryAfter);
            _runningJobs[jobKey] = runningJob;

            new Thread(
                 async () =>
                 {
                     try
                     {
                         using (var scope = _serviceScopeFactory.CreateScope())
                         {
                             var result = await job.Invoke(scope).ConfigureAwait(false);

                             var completedJob = _runningJobs[jobKey] as JobResponse<TErrorCode>;
                             completedJob.JobStatus = JobStatus.Completed;
                             SlideExpiration(completedJob);
                             completedJob.ErrorRecords.AddRange(result.ErrorRecords);

                             PurgeExpiredJobs();
                         }
                     }
                     catch (Exception exception)
                     {
                         var errorRecords = new ErrorRecords<TErrorCode>();
                         //errorRecords.Add(new ErrorRecord<TErrorCode>(exception));
                         runningJob.JobStatus = JobStatus.Completed;
                         //runningJob.ErrorRecords = errorRecords;
                     }
                 }).Start();

            AppendHeaders(runningJob.JobKey, runningJob.RetrySeconds);
            return ToStatusCode(runningJob, successCode: HttpStatusCode.Accepted);
        }

        private static void PurgeExpiredJobs()
        {
            var expiredJobKeys = _runningJobs.Where(x => x.Value.JobExpirationDateTime < DateTimeOffset.Now).Select(x => x.Key);

            foreach (var key in expiredJobKeys)
            {
                _runningJobs.TryRemove(key, out _);
            }
        }

        private static void SlideExpiration(IJobStatus job)
        {
            job.JobExpirationDateTime = DateTimeOffset.Now.AddSeconds(ExpirationFactor * job.RetrySeconds);
        }

        //// Must add this method to work with Dynamic type used in JobControllerBase.cs. It seems Dynamic can not find extension method without casting. Define directly in this class so that this method could be found in
        //// the call runningJob.ToStatusCode(202) in JobControllerBase.cs
        private static ObjectResult ToStatusCode(IJobStatus job, HttpStatusCode successCode = HttpStatusCode.OK, HttpStatusCode errorCode = HttpStatusCode.BadRequest)
        {
            if (job.HasError)
            {
                return new ObjectResult(job)
                {
                    StatusCode = (int)errorCode,
                };
            }
            else
            {
                return new ObjectResult(job)
                {
                    StatusCode = (int)successCode,
                };
            }
        }

        private void AppendHeaders(Guid jobKey, int retryAfter)
        {
            var checkStatusUrl = Url.Action(nameof(CheckStatus), null, null, Request.Scheme);

            Response.Headers.Append("location", $"{checkStatusUrl}?jobKey={jobKey}"); // Where the engine will poll to check status
            Response.Headers.Append("retry-after", retryAfter.ToString(CultureInfo.InvariantCulture));
        }
    }
}
