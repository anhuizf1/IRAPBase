using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    /// <summary>
    /// 操作类型DTO
    /// </summary>
    public class OpTypeDTO
    {

        /// <summary>
        /// 操作类型序号
        /// </summary>
        public byte OpType { get; set; }
        /// <summary>
        /// 操作类型代码
        /// </summary>
        public string OpTypeCode { get; set; }
        /// <summary>
        /// 操作类型名称
        /// </summary>
        public string OpTypeName { get; set; }
        /// <summary>
        /// 操作类型序号
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 辅助交易表
        /// </summary>
        public string AuxTranTBLName { get; set; }
        /// <summary>
        /// 临时事实表
        /// </summary>
        public string OLTPTempFactTBLName { get; set; }
        
        /// <summary>
        /// 固化事实表
        /// </summary>
        public string OLTPFixedFactTBLName { get; set; }
        /// <summary>
        /// 辅助事实表
        /// </summary>
        public string AuxFactTBLName { get; set; }
        /// <summary>
        /// 完成规则
        /// </summary>
        public string ComplementaryRule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StateExclCtrlStr { get; set; }
        /// <summary>
        /// 实体创建
        /// </summary>
        public bool EntityCreating { get; set; }
        /// <summary>
        /// 业务实际
        /// </summary>
        public bool BusinessDateIsValid { get; set; }
        /// <summary>
        /// 授权条件
        /// </summary>
        public string AuthorizingCondition { get; set; }
    }

    public class BizDimDTO  {

        public int Ordinal { get; set; }
        public int DimTreeID { get; set; }

        public string DimName { get; set; }
     }

    /// <summary>
    /// 业务度量
    /// </summary>
    public class BizMetDTO
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 度量名称
        /// </summary>
        public string MetricName { get; set; }
        /// <summary>
        /// 度量精度
        /// </summary>
        public byte Scale { get; set; }
        /// <summary>
        /// 度量单位
        /// </summary>
        public string UnitOfMeasure { get; set; }

    }

    /// <summary>
    /// 行集事实DTO
    /// </summary>
    public class BizRSFactDTO
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 行集表中文描述
        /// </summary>
        public string RSFactName { get; set; }
        /// <summary>
        /// 行集事实表名
        /// </summary>
        public string RSFactTBLName { get; set; }
        /// <summary>
        /// 行集事实保存
        /// </summary>
        public string ProcOnRSFactSave { get; set; }
        /// <summary>
        /// 行集追加
        /// </summary>
        public string ProcOnRSFactAppend { get; set; }
        /// <summary>
        /// 行集事实删除
        /// </summary>
        public string ProcOnRSFactDelete { get; set; }
    }

 
}
