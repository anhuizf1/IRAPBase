using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 行集事实表的基类，任何系统的行集事实表都应继承此类
    /// </summary>
    public class BaseRSFact : BaseEntity
    {
        public long FactID { get; set; }
        public long PartitioningKey { get; set; }
        public string WFInstanceID { get; set; }
        public int Ordinal { get; set; }
    }
}
