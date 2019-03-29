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

            Property(t => t.PartitioningKey).IsRequired();
            Property(t => t.NameID).IsRequired();
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.BChecksum).IsRequired();
            Property(t => t.NameDescription).IsRequired();
            Property(t => t.SearchCode1).IsRequired();
            Property(t => t.SearchCode2).IsRequired();
            Property(t => t.HelpMemoryCode).IsRequired();
        }
    }

    public class BizNamespaceMap : EntityTypeConfiguration<BizNameSpaceEntity>
    {

        public BizNamespaceMap()
        {
            //表定义

            HasKey(t => new { t.PartitioningKey, t.LanguageID, t.NameID });

            Property(t => t.PartitioningKey).IsRequired();
            Property(t => t.NameID).IsRequired();
            Property(t => t.LanguageID).IsRequired();
            Property(t => t.BChecksum).IsRequired();
            Property(t => t.NameDescription).IsRequired();
            Property(t => t.SearchCode1).IsRequired();
            Property(t => t.SearchCode2).IsRequired();
            Property(t => t.HelpMemoryCode).IsRequired();
        }
    }
}
