using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.mapping
{
    public class SystemMap: EntityTypeConfiguration<SystemEntity>
    {
        public SystemMap()
        {
            //表定义

            HasKey(t => t.SystemID);
            Property(t => t.Ordinal).IsRequired();
            Property(t => t.ProductNo).IsRequired();
            Property(t => t.Author).IsRequired();

            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
