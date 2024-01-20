namespace Framework.Business.Manager
{
    using AutoMapper;
    using EnsureThat;
    using Framework.Entity;
    using Microsoft.Extensions.Logging;
    using DataAccess.Repository;

    /// <summary>
    /// Defines the <see cref="ManagerBase" />.
    /// </summary>
    /// 
    public abstract class ManagerBase : IManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerBase"/> class.
        /// </summary>
        /// <param name="logger">The logger<see cref="ILogger"/>.</param>
        protected ManagerBase(ILogger logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));

            Logger = logger;
        }

        /// <summary>
        /// Gets the Logger.
        /// </summary>
        protected ILogger Logger { get; }
    }
}
