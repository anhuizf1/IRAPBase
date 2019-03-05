using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb035_Ex1")]
    public  class ModelTreeStatusList:BaseEntity
    {

        public Int16 TreeID { get; set; }
        public byte StatusIndex { get; set; }
    
        public byte Ordinal { get; set; }
        public string StatusName { get; set; }
    }
}
