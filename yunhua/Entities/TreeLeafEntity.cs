using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace IRAPBase.Entities
{

    public class TreeLeafEntity:BaseEntity
    {
        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }

        // [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LeafID { get; set; }
        public int NameID { get; set; }
        public string NodeName { get; set; }
        public int Father { get; set; }
        public float UDFOrdinal { get; set; }
        public byte NodeDepth { get; set; }
        public Int16 LeafStatus { get; set; }
        public int CSTRoot { get; set; }
        public int IconID { get; set; }
        public int EntityID { get; set; }
        public string Code { get; set; }
        public string AlternateCode { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    [Table("stb053")]
    public class ETreeSysLeaf : TreeLeafEntity
    {

    }

    [Table("stb058")]
    public class ETreeBizLeaf : TreeLeafEntity
    {

    }
    [Table("stb059")]
    public class ETreeRichLeaf : TreeLeafEntity
    {

    }
}
