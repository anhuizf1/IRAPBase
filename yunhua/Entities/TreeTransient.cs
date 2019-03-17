using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class TreeTransient : BaseEntity
    {
        public TreeTransient()
        {
            UnitOfMeasure = "";
        }
        public long PartitioningKey { get; set; }
        //public int TreeID { get; set; }
        public int EntityID { get; set; }
        public byte Ordinal { get; set; }

        public int RowChecksum { get; set; }
        public int VersionGE { get; set; }
        public int VersionLE { get; set; }
        public long AttrValue { get; set; }
       public Int16  Scale { get; set; }
       public string  UnitOfMeasure { get; set; }
    }
    /*老的结构
    public class TreeTransient:BaseEntity
    {
        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }
        public int EntityID { get; set; }
        public string Code { get; set; }
        public int BChecksum { get; set; }
        public int IconID { get; set; }
        public long EntityStatus { get; set; }
        public string DicingFilter { get; set; }
        public Int16 AttrCtrlValue { get; set; }
        public int RS01Version { get; set; }
        public int RS02Version { get; set; }
        public int RS03Version { get; set; }
        public int RS04Version { get; set; }
        public int RS05Version { get; set; }
        public int RS06Version { get; set; }
        public int RS07Version { get; set; }
        public int RS08Version { get; set; }
        public long Statistic01 { get; set; }
        public long Statistic02 { get; set; }
        public long Statistic03 { get; set; }
        public long Statistic04 { get; set; }
        public long Statistic05 { get; set; }
        public long Statistic06 { get; set; }
        public long Statistic07 { get; set; }
        public long Statistic08 { get; set; }
        public long Statistic09 { get; set; }
        public long Statistic10 { get; set; }
        public long Statistic11 { get; set; }
        public long Statistic12 { get; set; }
        public long Statistic13 { get; set; }
        public long Statistic14 { get; set; }
        public long Statistic15 { get; set; }
        public long Statistic16 { get; set; }
    }
    */
    [Table("stb187")]
    public class ETreeSysTran : TreeTransient
    {

    }

    [Table("stb188")]
    public class ETreeBizTran : TreeTransient
    {

    }

    [Table("stb187_H")]
    public class ETreeSysTran_H : TreeTransient
    {

    }

    [Table("stb188_H")]
    public class ETreeBizTran_H : TreeTransient
    {

    }

    /// <summary>
    /// 系统瞬态属性映射
    /// </summary>
    public class ETreeSysTranMap : EntityTypeConfiguration<ETreeSysTran>
    {

        public ETreeSysTranMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID ,t.Ordinal});

            Property(t => t.EntityID).IsRequired();
            Property(t => t.Ordinal).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
    /// <summary>
    /// 系统瞬态属性历史 映射
    /// </summary>
    public class ETreeSysTran_HMap : EntityTypeConfiguration<ETreeSysTran_H>
    {

        public ETreeSysTran_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });

            Property(t => t.EntityID).IsRequired();
            Property(t => t.Ordinal).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

    /// <summary>
    /// 业务瞬态属性映射
    /// </summary>
    public class ETreeBizTranMap : EntityTypeConfiguration<ETreeBizTran>
    {

        public ETreeBizTranMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID,t.Ordinal });

            Property(t => t.EntityID).IsRequired();
            Property(t => t.Ordinal).IsRequired();
            Property(t => t.AttrValue).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

    /// <summary>
    /// 业务瞬态属性历史 映射
    /// </summary>
    public class ETreeBizTran_HMap : EntityTypeConfiguration<ETreeBizTran_H>
    {
        public ETreeBizTran_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });

            Property(t => t.EntityID).IsRequired();
            Property(t => t.Ordinal).IsRequired();
            Property(t => t.AttrValue).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
