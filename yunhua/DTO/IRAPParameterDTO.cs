using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    /// <summary>
    /// 系统参数实体类
    /// </summary>
    public class IRAPParameterDTO
    {
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 参数标识号
        /// </summary>
        public byte ParameterID { get; set; }
        /// <summary>
        /// 参数名称标识号
        /// </summary>
        public int ParameterNameID { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数值（整型）
        /// </summary>
        public int ParameterValue { get; set; }
        /// <summary>
        /// 参数值（字符串）
        /// </summary>
        public string ParameterValueStr { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime TimeUpdated { get; set; }
    }
}
