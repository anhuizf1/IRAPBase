using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace IRAPBase.Entities
{

    public class TreeNodeEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }
        public int NodeID { get; set; }
        public string Code { get; set; }
        public int NameID { get; set; }
        public string NodeName { get; set; }
        public int Father { get; set; }
        public float UDFOrdinal { get; set; }
        public byte NodeDepth { get; set; }
        public byte NodeStatus { get; set; }
        public int CSTRoot { get; set; }
        public int IconID { get; set; }
        public int CntOfSonNodes { get; set; }
    }

    [Table("stb052")]
    public class ETreeSysDir : TreeNodeEntity
    {

    }

    [Table("stb057")]
    public class ETreeBizDir : TreeNodeEntity
    {

    }
    
}
