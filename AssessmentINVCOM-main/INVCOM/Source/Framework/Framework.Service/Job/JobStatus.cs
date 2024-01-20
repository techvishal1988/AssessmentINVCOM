namespace Framework.Service.Job
{
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum JobStatus
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
    {
        NotFound = 0,
        Running,  // don't change its name "Running". It is used in JobRunner.RunJobAsync(): while (result.JobStatus.ToString() == JobStatus.Running.ToString())
        Completed,
    }
}
