using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 辅助事实表的基类
    /// </summary>
    public class BaseAuxFact : BaseEntity
    {
        public BaseAuxFact()
        {
            WFInstanceID = "";
        }
        public long FactID { get; set; }
        public long PartitioningKey { get; set; }
        public long FactPartitioningKey { get; set; }
        public string WFInstanceID { get; set; }
    }

    /// <summary>
    /// 辅助交易表的基类
    /// </summary>
    public class BaseAuxTran: BaseEntity
    {
      /// <summary>
      /// 交易号
      /// </summary>
       public long TransactNo { get; set; }
        /// <summary>
        /// 分区键
        /// </summary>
       public long PartitioningKey { get; set; }

    }
}
