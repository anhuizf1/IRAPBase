using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IRAPBase.Entities
{

    // 旧的类库
    /*
    public class TreeClassEntity1:BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int AttrChangeID { get; set; }
        public Int16 TreeID { get; set; }
        public int LeafID { get; set; }
        public int AChecksum { get; set; }
        public DateTime SetTime { get; set; }
        public int SetSysLogID { get; set; }
        public long TransactNoGT { get; set; }
        public long TransactNoLE { get; set; }
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
        public int Leaf11 { get; set; }
        public int Leaf12 { get; set; }
        public int Leaf13 { get; set; }
        public int Leaf14 { get; set; }
        public int Leaf15 { get; set; }
        public int Leaf16 { get; set; }
        public int Leaf17 { get; set; }
        public int Leaf18 { get; set; }
        public int Leaf19 { get; set; }
        public int Leaf20 { get; set; }
        public int Leaf21 { get; set; }
        public int Leaf22 { get; set; }
        public int Leaf23 { get; set; }
        public int Leaf24 { get; set; }
    }
   */

    public class TreeClassEntity : BaseEntity
    {
        public TreeClassEntity()
        {
            A4Code = "";
            A4AlternateCode = "";
            A4DescInEnglish = "";
            A4NodeName = "";
        }
        public long PartitioningKey { get; set; }
        //public Int16 TreeID { get; set; }
        public int LeafID { get; set; }
        public int Ordinal { get; set; }
        public int RowChecksum { get; set; }
        public int VersionGE { get; set; }
        public int VersionLE { get; set; }
        public long TransactNoGT { get; set; }
        public long TransactNoLE { get; set; }
        public Int16 AttrTreeID { get; set; }
        public byte NodeDepth { get; set; }
        public int CSTRoot { get; set; }
        public int A4LeafID { get; set; }
        public string A4Code { get; set; }
        public string A4AlternateCode { get; set; }
        //public string A4NameID { get; set; }
        public string A4NodeName { get; set; }
        public string A4DescInEnglish { get; set; }
        public long MDMLogID { get; set; }
    }

    [Table("stb197")]
    public class ETreeSysClass : TreeClassEntity
    {

    }

    [Table("stb198")]
    public class ETreeBizClass : TreeClassEntity
    {

    }

    [Table("stb197_H")]
    public class ETreeSysClass_H : TreeClassEntity
    {

    }

    [Table("stb198_H")]
    public class ETreeBizClass_H : TreeClassEntity
    {

    }

    /*
    [Table("stb063")]
    public class ETreeSysClass1 : TreeClassEntity1
    {

    }


    [Table("stb064")]
    public class ETreeBizClass1 : TreeClassEntity1
    {

    }*/
    //映射类
    public class ETreeSysClassMap : EntityTypeConfiguration<ETreeSysClass>
    {

        public ETreeSysClassMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID, t.Ordinal });
           // Property(t => t.TreeID).IsRequired();
            Property(t => t.A4LeafID).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    public class ETreeBizClassMap : EntityTypeConfiguration<ETreeBizClass>
    {

        public ETreeBizClassMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID, t.Ordinal });

            Property(t => t.Ordinal).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    //映射历史表
    public class ETreeSysClass_HMap : EntityTypeConfiguration<ETreeSysClass_H>
    {

        public ETreeSysClass_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID, t.Ordinal,t.VersionLE });
           // Property(t => t.TreeID).IsRequired();
            Property(t => t.A4LeafID).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
    public class ETreeBizClass_HMap : EntityTypeConfiguration<ETreeBizClass_H>
    {

        public ETreeBizClass_HMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID, t.Ordinal ,t.VersionLE});

            Property(t => t.Ordinal).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


}
