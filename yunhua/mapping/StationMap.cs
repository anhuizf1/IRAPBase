using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.mapping
{
    public class StationMap: EntityTypeConfiguration<StationEntity>
    {
        public StationMap()
        {
            //表定义
             ToTable("stb008");
            HasKey(t =>  t.StationID  );

            Property(t => t.HostName).IsRequired();
            Property(t => t.StationID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
