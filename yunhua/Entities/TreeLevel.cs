using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace IRAPBase.Entities
{
    [Table("stb165")]
    public class TreeLevel
    {
        public long PartitioningKey { get; set; }
        public int TreeID { get; set; }
        public byte NodeDepth { get; set; }
        public int LevelAliasNameID { get; set; }
        public int DefaultIconID { get; set; }
    }
}
