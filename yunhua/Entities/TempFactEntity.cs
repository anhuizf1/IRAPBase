using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class FactEntity
    {
        public long FactID { get; set; }
        public long PartitioningKey { get; set; }
        public long TransactNo { get; set; }
        public byte IsFixed { get; set; }
        public int OpID { get; set; }
        public byte OpType { get; set; }
        public DateTime BusinessDate { get; set; }
        public string Code01 { get; set; }
        public string Code02 { get; set; }
        public string Code03 { get; set; }
        public string Code04 { get; set; }
        public string Code05 { get; set; }
        public string Code06 { get; set; }
        public string Code07 { get; set; }
        public string Code08 { get; set; }
        public string Code09 { get; set; }
        public string Code10 { get; set; }
        public int Leaf01 { get; set; }
        public int Leaf02 { get; set; }
        public int Leaf03 { get; set; }
        public int Leaf04 { get; set; }
        public int Leaf05 { get; set; }
        public int Leaf06 { get; set; }
        public int Leaf07 { get; set; }
        public int Leaf08 { get; set; }
        public int Leaf09 { get; set; }
        public int Leaf10 { get; set; }
        public int AChecksum { get; set; }
        public int CorrelationID { get; set; }
        public long Metric01 { get; set; }
        public long Metric02 { get; set; }
        public long Metric03 { get; set; }
        public long Metric04 { get; set; }
        public long Metric05 { get; set; }
        public long Metric06 { get; set; }
        public long Metric07 { get; set; }
        public long Metric08 { get; set; }
        public long Metric09 { get; set; }
        public long Metric10 { get; set; }
        public long Metric11 { get; set; }
        public long Metric12 { get; set; }
        public int BChecksum { get; set; }
        public int MeasurementID { get; set; }
        public string WFInstanceID { get; set; }
        public string Remark { get; set; }
        public long LinkedFactID { get; set; }
    }
}
