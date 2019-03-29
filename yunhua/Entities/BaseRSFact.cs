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
        /// <summary>
        /// 事实编号（通过序列服务器申请）
        /// </summary>
        public long FactID { get; set; }
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 工作流实例
        /// </summary>
        public string WFInstanceID { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Ordinal { get; set; }
    }
}
