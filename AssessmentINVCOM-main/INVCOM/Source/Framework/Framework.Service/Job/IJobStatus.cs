namespace Framework.Service;

using Framework.Service.Job;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IJobStatus
{
    DateTimeOffset? JobStartDateTime { get; set; }

    DateTimeOffset? JobExpirationDateTime { get; set; }

    int RetrySeconds { get; set; }

    Guid JobKey { get; set; }

    JobStatus JobStatus { get; set; }

    bool HasError { get; }
}

public interface IJobStatus<TErrorCode> : IJobStatus
    where TErrorCode : struct, Enum
{
    public ErrorRecords<TErrorCode> ErrorRecords { get; set; }
}
