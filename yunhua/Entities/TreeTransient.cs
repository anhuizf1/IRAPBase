using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class TreeTransient:BaseEntity
    {
        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }
        public int EntityID { get; set; }
        public string Code { get; set; }
        public int BChecksum { get; set; }
        public int IconID { get; set; }
        public long EntityStatus { get; set; }
        public string DicingFilter { get; set; }
        public Int16 AttrCtrlValue { get; set; }
        public int RS01Version { get; set; }
        public int RS02Version { get; set; }
        public int RS03Version { get; set; }
        public int RS04Version { get; set; }
        public int RS05Version { get; set; }
        public int RS06Version { get; set; }
        public int RS07Version { get; set; }
        public int RS08Version { get; set; }
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
        public long Statistic11 { get; set; }
        public long Statistic12 { get; set; }
        public long Statistic13 { get; set; }
        public long Statistic14 { get; set; }
        public long Statistic15 { get; set; }
        public long Statistic16 { get; set; }
    }

    [Table("stb054")]
    public class ETreeSysTran : TreeTransient
    {

    }

    [Table("stb060")]
    public class ETreeBizTran : TreeTransient
    {

    }


}
