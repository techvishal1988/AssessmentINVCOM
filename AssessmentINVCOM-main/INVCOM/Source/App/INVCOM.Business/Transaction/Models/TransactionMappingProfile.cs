namespace INVCOM.Business.Transaction.Models
{
    using Amazon.Util;
    using AutoMapper;
    using INVCOM.Entity.Entities;
    using System;


    /// <summary>
    /// Defines the <see cref="TransactionMappingProfile" />.
    /// </summary>
    public class TransactionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionMappingProfile"/> class.
        /// </summary>
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, TransactionReadModel>();

            CreateMap<TransactionCreateModel, Transaction>()
                .ForMember(x => x.ReferenceNumber, opt => opt.Ignore())
                .ForMember(x => x.TransactionDate, opt => opt.MapFrom(a=>a.TransactionDate.ToString(AWSSDKUtils.ISO8601DateFormat)))
                .ForMember(x => x.TransactionUpdatedDate, opt => opt.MapFrom(a=>a.TransactionDate.ToString(AWSSDKUtils.ISO8601DateFormat)));

            CreateMap<TransactionUpdateModel, Transaction>()
                .ForMember(x => x.IsSynced, opt => opt.MapFrom(a=> true))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(a => true))
                .ForMember(x => x.TransactionUpdatedDate, opt => opt.MapFrom(a => DateTime.UtcNow.ToString(AWSSDKUtils.ISO8601DateFormat)));
        }
    }
}
