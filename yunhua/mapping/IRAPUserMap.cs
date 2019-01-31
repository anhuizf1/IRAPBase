using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPBase.mapping
{
  
        public class IRAPUserMap : EntityTypeConfiguration<IRAPUserEntity>
        {

            public IRAPUserMap()
            {
                //表定义
                ToTable("stb006");
                HasKey(t => new { t.PartitioningKey, t.UserCode });

                Property(t => t.UserCode).IsRequired();
                Property(t => t.UserName).IsRequired();
                Property(t => t.EncryptedPWD).IsRequired();

              //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }


 
}
