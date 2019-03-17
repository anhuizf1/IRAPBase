using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb010")]
    public class TransactEntity:BaseEntity
    {
        public long TransactNo { get; set; }
        public long PartitioningKey { get; set; }
        public DateTime OperTime { get; set; }
        public DateTime? OkayTime { get; set; }
        public DateTime? RevokeTime { get; set; }
        public int AgencyLeaf1 { get; set; }
        public int AgencyLeaf2 { get; set; }
        public int AgencyLeaf3 { get; set; }
        public string Operator { get; set; }
        public string Checked { get; set; }
        public string Revoker { get; set; }
        public string StationID { get; set; }
        public string IPAddress { get; set; }
        public string OpNodes { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherNoEx { get; set; }
        public int Attached1 { get; set; }
        public int Attached2 { get; set; }
        public int T16LeafID { get; set; }
        public string WFInstanceID { get; set; }
        public long LinkedTransactNo { get; set; }
        public byte Status { get; set; }
        public string Remark { get; set; }
    }
}
