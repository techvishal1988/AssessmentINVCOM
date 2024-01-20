namespace Framework.Service.Job
{
   
    using System;


    public class JobResponse<TErrorCode> : ManagerResponse<TErrorCode>, IJobStatus
        where TErrorCode : struct, Enum
    {
        public JobResponse()
        {
        }

        public JobResponse(Guid jobKey, JobStatus jobStatus, DateTimeOffset? jobStartDateTime, DateTimeOffset? jobExpirationDateTime, int retrySeconds = 30)
        {
            JobKey = jobKey;
            JobStatus = jobStatus;
            JobStartDateTime = jobStartDateTime;
            RetrySeconds = retrySeconds;
            JobExpirationDateTime = jobExpirationDateTime;
        }

        public DateTimeOffset? JobStartDateTime { get; set; }

        public DateTimeOffset? JobExpirationDateTime { get; set; }

        // Don't change this property name. This name is used in JobRunner.cs: public const string RetrySeconds = "RetrySeconds";
        public int RetrySeconds { get; set; }

        // Don't change this property name. This name is used in JobRunner.cs: public const string JobKey = "JobKey";
        public Guid JobKey { get; set; }

        // Don't change this property name. This name is used in JobRunner.cs: public const string JobStatusPropertyName = "JobStatus";
        public JobStatus JobStatus { get; set; }

        //public bool HasError => throw new NotImplementedException();
    }
}
