using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
   [Table("stb112")]
    public class ModelTreeClassfiyEntity : BaseEntity
    {
        public Int16 TreeID { get; set; }
        public byte AttrIndex { get; set; }
        public Int16 AttrTreeID { get; set; }
        public int AttrNameID { get; set; }
        public string DicingFilter { get; set; }
        public bool SaveChangeHistory { get; set; }
        public byte NodeDepth { get; set; }
        public bool Required { get; set; }
       
    }
}
