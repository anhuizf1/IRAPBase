using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 状态属性实体基类
    /// </summary>
    public class TreeStatus : BaseEntity
    {
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        // public Int16 TreeID { get; set; }
        /// <summary>
        /// 实体标识
        /// </summary>
        public int EntityID { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 第5棵树叶标识
        /// </summary>
        public int T5LeafID { get; set; }
        /// <summary>
        /// 状态值
        /// </summary>
        public byte StatusValue { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public int ColorRGBValue { get; set; }
        /// <summary>
        /// 转换控制值
        /// </summary>
        public long TransitCtrlValue { get; set; }
    }

    /// <summary>
    /// 系统状态属性实体
    /// </summary>
    [Table("stb177")]
    public class ETreeSysStatus : TreeStatus
    {

    }

    /// <summary>
    /// 业务状态属性实体
    /// </summary>
    [Table("stb178")]
    public class ETreeBizStatus : TreeStatus
    {

    }

    [Table("stb177_H")]
    public class ETreeSysStatus_H : TreeStatus
    {

    }

    [Table("stb178_H")]
    public class ETreeBizStatus_H : TreeStatus
    {

    }
    public class ETreeSysStatusMap : EntityTypeConfiguration<ETreeSysStatus>
    {

        public ETreeSysStatusMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });

            Property(t => t.StatusValue).IsRequired();
            Property(t => t.T5LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    public class ETreeBizStatusMap : EntityTypeConfiguration<ETreeBizStatus>
    {

        public ETreeBizStatusMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });

            Property(t => t.StatusValue).IsRequired();
            Property(t => t.T5LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    //历史表映射
    public class ETreeSysStatus_HMap : EntityTypeConfiguration<ETreeSysStatus_H>
    {

        public ETreeSysStatus_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });

            Property(t => t.StatusValue).IsRequired();
            Property(t => t.T5LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    public class ETreeBizStatus_HMap : EntityTypeConfiguration<ETreeBizStatus_H>
    {

        public ETreeBizStatus_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });
            Property(t => t.StatusValue).IsRequired();
            Property(t => t.T5LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
