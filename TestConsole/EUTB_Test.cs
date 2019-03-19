using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace TestConsole
{
    public class EUTB_Test : BaseEntity
    {
        public int UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }

    }


    public class EUTB_TestMap : EntityTypeConfiguration<EUTB_Test>
    {

        public EUTB_TestMap()
        {
            //表定义
            ToTable("UTB_Test");
            HasKey(t => t.UserID);
            Property(t => t.UserName).IsRequired();
            Property(t => t.UserCode).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
