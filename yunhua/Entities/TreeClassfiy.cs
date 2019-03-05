﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace IRAPBase.Entities
{
    public class TreeClassEntity:BaseEntity
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


    [Table("stb063")]
    public class ETreeSysClass : TreeClassEntity
    {

    }

    [Table("stb064")]
    public class ETreeBizClass : TreeClassEntity
    {

    }

    //映射类
    public class ETreeSysClassMap : EntityTypeConfiguration<ETreeSysClass>
    {

        public ETreeSysClassMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID });

            Property(t => t.TransactNoLE).IsRequired();
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
            HasKey(t => new { t.PartitioningKey, t.LeafID });

            Property(t => t.TransactNoLE).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}