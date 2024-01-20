using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVCOM.DataAccess.Repository.Model
{
    public class TransactionSearchModel
    {
        public TransactionSearchModel()
        {
            CustomerIds = new List<long>();
        }
        public List<long> CustomerIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid ReferenceNumber { get; set; }
    }
}
