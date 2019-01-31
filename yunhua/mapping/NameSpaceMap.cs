using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.mapping
{
    public class SysNamespaceMap : EntityTypeConfiguration<SysNameSpaceEntity>
    {

        public SysNamespaceMap()
        {
            //表定义
          
            HasKey(t => new { t.PartitioningKey, t.LanguageID, t.NameID });

            Property(t => t.NameDescription).IsRequired();
            Property(t => t.NameID).IsRequired();
            Property(t => t.LanguageID).IsRequired();

            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

    public class BizNamespaceMap : EntityTypeConfiguration<BizNameSpaceEntity>
    {

        public BizNamespaceMap()
        {
            //表定义

            HasKey(t => new { t.PartitioningKey, t.LanguageID, t.NameID });

            Property(t => t.NameDescription).IsRequired();
            Property(t => t.NameID).IsRequired();
            Property(t => t.LanguageID).IsRequired();

            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
