using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPMES.Entities
{
    public class Eutb_Users : BaseEntity
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
    }


    public class Eutb_UsersMap : EntityTypeConfiguration<Eutb_Users>
    {
        public Eutb_UsersMap()
        {
            //表定义
            ToTable("utb_Users");
            HasKey(t => t.UserCode);
           // Property(p => p.Age).HasColumnType("smallint");
        }
    }

}