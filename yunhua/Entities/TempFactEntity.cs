using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 临时主事实表
    /// </summary>
    [Table("TempFact_OLTP")]
    public class FactEntity:BaseEntity
    {
        /// <summary>
        /// 构造函数初始化字段
        /// </summary>
        public FactEntity()
        {
            Code01 = "";
            Code02 = "";
            Code03 = "";
            Code04 = "";
            Code05 = "";
            Code06 = "";
            Code07 = "";
            Code08 = "";
            Code09 = "";
            Code10 = "";
            WFInstanceID = "";
            Remark = "";
        }
        /// <summary>
        /// 事实编号
        /// </summary>
        public long FactID { get; set; }
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public long TransactNo { get; set; }
        /// <summary>
        /// 是否固化
        /// </summary>
        public byte IsFixed { get; set; }
        /// <summary>
        /// 业务操作
        /// </summary>
        public int OpID { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public byte OpType { get; set; }
        /// <summary>
        /// 业务发生时间
        /// </summary>
        public DateTime BusinessDate { get; set; }
        /// <summary>
        /// 维度代码01
        /// </summary>
        public string Code01 { get; set; }
        /// <summary>
        /// 维度代码02
        /// </summary>
        public string Code02 { get; set; }
        /// <summary>
        /// 维度代码03
        /// </summary>
        public string Code03 { get; set; }
        /// <summary>
        /// 维度代码04
        /// </summary>
        public string Code04 { get; set; }
        /// <summary>
        /// 维度代码05
        /// </summary>
        public string Code05 { get; set; }
        /// <summary>
        /// 维度代码06
        /// </summary>
        public string Code06 { get; set; }
        /// <summary>
        /// 维度代码07
        /// </summary>
        public string Code07 { get; set; }
        /// <summary>
        /// 维度代码08
        /// </summary>
        public string Code08 { get; set; }
        /// <summary>
        /// 维度代码09
        /// </summary>
        public string Code09 { get; set; }
        /// <summary>
        /// 维度代码10
        /// </summary>
        public string Code10 { get; set; }
        /// <summary>
        /// 维度标识01
        /// </summary>
        public int Leaf01 { get; set; }
        public int Leaf02 { get; set; }
        public int Leaf03 { get; set; }
        public int Leaf04 { get; set; }
        public int Leaf05 { get; set; }
        public int Leaf06 { get; set; }
        public int Leaf07 { get; set; }
        public int Leaf08 { get; set; }
        public int Leaf09 { get; set; }
        public int Leaf10 { get; set; }
        public int AChecksum { get; set; }
        public int CorrelationID { get; set; }

        /// <summary>
        /// 度量01
        /// </summary>
        public long Metric01 { get; set; }
        /// <summary>
        /// 度量02
        /// </summary>
        public long Metric02 { get; set; }
        /// <summary>
        /// 度量03
        /// </summary>
        public long Metric03 { get; set; }
        /// <summary>
        /// 度量04
        /// </summary>
        public long Metric04 { get; set; }
        /// <summary>
        /// 度量05
        /// </summary>
        public long Metric05 { get; set; }
        /// <summary>
        /// 度量06
        /// </summary>
        public long Metric06 { get; set; }
        /// <summary>
        /// 度量07
        /// </summary>
        public long Metric07 { get; set; }
        /// <summary>
        /// 度量08
        /// </summary>
        public long Metric08 { get; set; }
        /// <summary>
        /// 度量09
        /// </summary>
        public long Metric09 { get; set; }
        public long Metric10 { get; set; }
        public long Metric11 { get; set; }
        public long Metric12 { get; set; }
        /// <summary>
        /// 度量校验和
        /// </summary>
        public int BChecksum { get; set; }
        /// <summary>
        /// 度量关联
        /// </summary>
        public int MeasurementID { get; set; }
        /// <summary>
        /// 工作流实例
        /// </summary>
        public string WFInstanceID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 关联事实编号
        /// </summary>
        public long LinkedFactID { get; set; }
    }

    public class FactEntityMap : EntityTypeConfiguration<FactEntity>
    {
        public FactEntityMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey,t.FactID });
            Property(t => t.TransactNo).IsRequired();
            Property(t => t.Remark).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

}
