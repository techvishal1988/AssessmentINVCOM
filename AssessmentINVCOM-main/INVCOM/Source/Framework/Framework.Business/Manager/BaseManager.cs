using AutoMapper;
using EnsureThat;
using Framework.DataAccess.Repository;
using Framework.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Business.Manager
{
    public abstract class BaseManager<TEntity> : ManagerBase
        where TEntity : BaseEntity
    {

        protected IGenericRepository<TEntity> Repository { get; private set; }

        protected IMapper Mapper { get; }

        protected BaseManager(IGenericRepository<TEntity> genericRepository, ILogger<BaseManager<TEntity>> logger, IMapper mapper)
            : base(logger)
        {
            EnsureArg.IsNotNull(genericRepository, nameof(genericRepository));
            EnsureArg.IsNotNull(mapper, nameof(mapper));
            Mapper = mapper;
            Repository = genericRepository;
        }

       
        
    }
}
