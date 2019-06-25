using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 关联属性
    /// </summary>
    public class BaseCorrAttr : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int CorrelationID { get; set; }
        public long CorrStatus { get; set; }
        public int AChecksum { get; set; }
        public long Statistic01 { get; set; }
        public long Statistic02 { get; set; }
        public long Statistic03 { get; set; }
        public long Statistic04 { get; set; }
        public long Statistic05 { get; set; }
        public long Statistic06 { get; set; }
        public long Statistic07 { get; set; }
        public long Statistic08 { get; set; }
        public long Statistic09 { get; set; }

        public long Statistic10 { get; set; }

        public int RS01Version { get; set; }
        public int RS02Version { get; set; }
        public int RS03Version { get; set; }
        public int RS04Version { get; set; }
        public int RS05Version { get; set; }
        public int RS06Version { get; set; }
        public int RS07Version { get; set; }
        public int RS08Version { get; set; }

    }

    public class BaseCorrRSAttr : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int CorrelationID { get; set; }
        public int Ordinal { get; set; }
        public int RowChecksum { get; set; }
        public int VersionGE { get; set; }
        public int VersionLE { get; set; }
    }

}
