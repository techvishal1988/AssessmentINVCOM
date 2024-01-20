using System.Collections.Generic;

namespace Framework.DataAccess.Model
{
    public class PagedResultModel<T>
    {
        public List<T> Items { get; }
        public string LastEvaluatedKey { get; }
        public bool HasMorePages { get; }

        public PagedResultModel(List<T> items, string lastEvaluatedKey, bool hasMorePages)
        {
            Items = items;
            LastEvaluatedKey = lastEvaluatedKey;
            HasMorePages = hasMorePages;
        }
    }
}
