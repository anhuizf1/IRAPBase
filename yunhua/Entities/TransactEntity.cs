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
   /// 交易表实体
   /// </summary>
    [Table("stb010")]
    public class TransactEntity:BaseEntity
    {
        /// <summary>
        /// 构造函数，初始化字段
        /// </summary>
        public TransactEntity()
        {
            Checked = "";
            Revoker = "";
            StationID = "";
            IPAddress = "";
            VoucherNo = "";
            VoucherNoEx = "";
            WFInstanceID = "";
            Remark = "";
        }
        /// <summary>
        /// 交易号
        /// </summary>
        public long TransactNo { get; set; }
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperTime { get; set; }
        /// <summary>
        /// 复核时间
        /// </summary>
        public DateTime? OkayTime { get; set; }
        /// <summary>
        /// 撤销时间
        /// </summary>
        public DateTime? RevokeTime { get; set; }
        /// <summary>
        /// 操作机构
        /// </summary>
        public int AgencyLeaf1 { get; set; }
        /// <summary>
        /// 复核机构
        /// </summary>
        public int AgencyLeaf2 { get; set; }
        /// <summary>
        /// 撤销机构
        /// </summary>
        public int AgencyLeaf3 { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 复核人员
        /// </summary>
        public string Checked { get; set; }
        /// <summary>
        /// 撤销人员
        /// </summary>
        public string Revoker { get; set; }
        /// <summary>
        /// 站点编号MAC地址
        /// </summary>
        public string StationID { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 业务操作集合（第4棵树负的树叶子，如果多条用逗号 隔开)
        /// </summary>
        public string OpNodes { get; set; }
        /// <summary>
        /// 票据号
        /// </summary>
        public string VoucherNo { get; set; }
        /// <summary>
        /// 票据号扩展
        /// </summary>
        public string VoucherNoEx { get; set; }
        /// <summary>
        /// 附件张数1
        /// </summary>
        public int Attached1 { get; set; }
        /// <summary>
        /// 附件张数2
        /// </summary>
        public int Attached2 { get; set; }
        /// <summary>
        /// 附件张数3
        /// </summary>
        public int T16LeafID { get; set; }
        /// <summary>
        /// 工作流实例
        /// </summary>
        public string WFInstanceID { get; set; }
        /// <summary>
        /// 关联交易号
        /// </summary>
        public long LinkedTransactNo { get; set; }
        /// <summary>
        /// 交易状态 0-未复核 1-已写事实未复核 3-已复核 4-已撤销5-已固化
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }


    public class TransactEntityMap : EntityTypeConfiguration<TransactEntity>
    {

        public TransactEntityMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.TransactNo });
            Property(t => t.Operator).IsRequired();
            Property(t => t.OperTime).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
