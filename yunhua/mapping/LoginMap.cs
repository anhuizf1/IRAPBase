using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.mapping
{
   public  class LoginMap: EntityTypeConfiguration<LoginEntity>
    {
        public LoginMap()
        {
            //表定义
            ToTable("stb009");
            HasKey(t => t.SysLogID);
            Property(t => t.UserCode).IsRequired();
            Property(t => t.AgencyLeaf).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
