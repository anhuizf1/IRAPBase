using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IRAPBase.Entities
{
    public class TreeStatus : BaseEntity
    {

        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }
        public int EntityID { get; set; }

        public int Ordinal { get; set; }
        public int T5LeafID { get; set; }
        public byte StatusValue { get; set; }
    }

    [Table("stb177")]
    public class ETreeSysStatus : TreeStatus
    {

    }

    [Table("stb178")]
    public class ETreeBizStatus : TreeStatus
    {

    }

    public class ETreeSysStatusMap : EntityTypeConfiguration<ETreeSysStatus>
    {

        public ETreeSysStatusMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey,  t.EntityID,t.Ordinal });

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
}
