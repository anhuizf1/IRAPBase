using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class DimEntity
    {
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
        public int Leaf11 { get; set; }
        public int Leaf12 { get; set; }
    }
    public class MetricEntity
    {
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
    }

    public class SequenceValueDTO
    {
        public int ErrCode { get; set; }
        public string ErrText { get; set; }

        public long SequenceValue { get; set; }
    }
}
