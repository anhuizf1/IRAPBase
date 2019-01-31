using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
 

    public class NameSpaceEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int NameID { get; set; }
        public Int16 LanguageID { get; set; }
        public int BChecksum { get; set; }
        public string NameDescription { get; set; }
        public string SearchCode1 { get; set; }
        public string SearchCode2 { get; set; }
        public string HelpMemoryCode { get; set; }
    }

    [Table("stb003")]
    public class SysNameSpaceEntity : NameSpaceEntity
    {

    }

    [Table("stb004")]
    public class BizNameSpaceEntity : NameSpaceEntity
    {

    }
}
