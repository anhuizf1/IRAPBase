using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class BaseRowAttrEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int EntityID { get; set; }
        public int Ordinal { get; set; }
        public int RowChecksum { get; set; }
        public int VersionGE { get; set; }
        public int VersionLE { get; set; }
    }
}
