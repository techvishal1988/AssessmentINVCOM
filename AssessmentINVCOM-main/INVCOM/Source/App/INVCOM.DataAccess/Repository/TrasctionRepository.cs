namespace INVCOM.DataAccess.Repository
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Util;
    using Framework.DataAccess.Repository;
    using INVCOM.DataAccess.Repository.Model;
    using INVCOM.Entity.Entities;
    using LinqKit;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TrasctionRepository" />.
    /// </summary>
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly IAmazonDynamoDB _client;
        public TransactionRepository(IDynamoDBContext dbContext, IAmazonDynamoDB client) : base(dbContext, client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Transaction>> SearchTransactionByFilter(TransactionSearchModel searchModel)
        {

            var filterFields = GetFilterFields(searchModel);
            var filterExpression = GetFilterExpr(searchModel);
            var request = new ScanRequest
            {
                TableName = "InvTransaction"

            };

            if (!filterExpression.IsNullOrEmpty())
            {
                request.FilterExpression = filterExpression;
                var attrNames = new Dictionary<string, string>();
                filterFields.ForEach(a => attrNames.Add($"#{a}", a));
                request.ExpressionAttributeNames = attrNames;

                var attrValues = new Dictionary<string, AttributeValue>();
                foreach (var attr in filterFields)
                {
                    if (attr == nameof(Transaction.CustomerId))
                    {
                        for (int i = 1; i <= searchModel.CustomerIds.Count; i++)
                        {
                            attrValues.Add($":customerId{i}", new AttributeValue { N = searchModel.CustomerIds[i - 1].ToString() });
                        }
                    }

                    if (attr == nameof(Transaction.TransactionDate))
                    {
                        if (searchModel.FromDate != null)
                        {
                            attrValues.Add($":fromDate", new AttributeValue { S = searchModel.FromDate.Value.ToString(AWSSDKUtils.ISO8601DateFormat) });

                        }
                        if (searchModel.ToDate != null)
                        {
                            attrValues.Add($":toDate", new AttributeValue { S = searchModel.ToDate.Value.ToString(AWSSDKUtils.ISO8601DateFormat) });
                        }
                    }
                    else if (attr == nameof(Transaction.ReferenceNumber))
                    {
                        attrValues.Add($":referenceNumber", new AttributeValue { S = searchModel.ReferenceNumber.ToString() });
                    }

                }

                request.ExpressionAttributeValues = attrValues;
            }
            try
            {
                var response = await _client.ScanAsync(request);
                var items = response.Items.Select(a =>
                {
                    var doc = Document.FromAttributeMap(a);
                    return _dynamoDBContext.FromDocument<Transaction>(doc);
                }).ToList();


                return items;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
            

          

        private IEnumerable<string> GetFilterFields(TransactionSearchModel searchModel)
        {

            var filterFields = new List<string>();
            if (searchModel.CustomerIds.Any())
            {
                filterFields.Add(nameof(Transaction.CustomerId));
            }

            if (searchModel.FromDate != null || searchModel.ToDate != null)
            {
                filterFields.Add(nameof(Transaction.TransactionDate));
            }

            if (searchModel.ReferenceNumber != System.Guid.Empty)
            {
                filterFields.Add(nameof(Transaction.ReferenceNumber));
            }
            

            return filterFields;
        }
        private string GetFilterExpr(TransactionSearchModel searchModel)
        {
            
            StringBuilder filterBuilder = new StringBuilder();
            if (searchModel.CustomerIds.Any())
            {
                var customerIds = new List<string>();
                for (int i = 1; i <= searchModel.CustomerIds.Count; i++)
                {
                    customerIds.Add($":customerId{i}");
                }
                filterBuilder.Append($"#CustomerId IN ({string.Join(",",customerIds)})");
                filterBuilder.Append(" AND ");

            }
            

            if (searchModel.FromDate != null && searchModel.ToDate != null)
            {
                filterBuilder.Append($"(#TransactionDate BETWEEN :fromDate AND :toDate)");
            }
            else if (searchModel.FromDate != null)
            {
                filterBuilder.Append($"#TransactionDate >= :fromDate");
            }
            else if (searchModel.ToDate != null)
            {
                filterBuilder.Append($"#TransactionDate <= :toDate");
            }

            if (searchModel.FromDate != null || searchModel.ToDate != null)
            {
                filterBuilder.Append(" AND ");
            }

            if (searchModel.ReferenceNumber != System.Guid.Empty)
            {
                filterBuilder.Append($"#ReferenceNumber = :referenceNumber");
            }

            var filterString = filterBuilder.ToString();

            if (filterString.TrimEnd().EndsWith("AND"))
            {
                filterString = filterString.Substring(0, filterString.LastIndexOf("AND"));                
            }

            return filterString;
        }

        public async Task<IEnumerable<Transaction>> GetActiveTransaction()
        {
            var expressionAttributeNames = new Dictionary<string, string> 
            {
                { "#IsActive", "IsActive" },
                { "#IsSynced", "IsSynced" }
            };

            string filterExpression = "boolAttribute = :false";
            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                 { ":true", new AttributeValue { BOOL = true} },
                { ":false", new AttributeValue { BOOL = true} }
            };

            var request = new ScanRequest
            {
                TableName = "InvTransaction",
                ExpressionAttributeNames = expressionAttributeNames,
                ExpressionAttributeValues = expressionAttributeValues,
                //new Dictionary<string, AttributeValue>
                //{
                //    {":isActive", new AttributeValue { BOOL = true}},
                //    {":isSynced", new AttributeValue { BOOL = false }}
                //},
                FilterExpression = "#IsActive = :true AND #IsSynced = :false",
                //ProjectionExpression = "ReferenceNumber"
            };

            var response = await _client.ScanAsync(request).ConfigureAwait(false);

            return default;
        }

    }
}
