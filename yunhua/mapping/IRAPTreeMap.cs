using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPBase.mapping
{
    public class SysTreeDirMap : EntityTypeConfiguration<ETreeSysLeaf>
    {

        public SysTreeDirMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID });

            Property(t => t.LeafID).IsRequired();
            Property(t => t.EntityID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    public class SysTreeNodeMap : EntityTypeConfiguration<ETreeSysDir>
    {

        public SysTreeNodeMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.NodeID });

            Property(t => t.NodeName).IsRequired();
            Property(t => t.NodeID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


    public class ETreeBizDirMap : EntityTypeConfiguration<ETreeBizDir>
    {

        public ETreeBizDirMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.NodeID });

            Property(t => t.NodeName).IsRequired();
            Property(t => t.NodeID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

    public class ETreeBizLeafMap : EntityTypeConfiguration<ETreeBizLeaf>
    {

        public ETreeBizLeafMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.LeafID });

            Property(t => t.NodeName).IsRequired();
            Property(t => t.LeafID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
