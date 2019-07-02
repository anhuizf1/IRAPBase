using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb031")]
    public class GenAttr_T1 : BaseGenAttrEntity
    {
        public string BriefDesc { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Chief { get; set; }
        public string ToContact { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string CountryCode { get; set; }
        public string MemoText { get; set; }

    }


    public class GenAttr_T1Map : EntityTypeConfiguration<GenAttr_T1>
    {

        public GenAttr_T1Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID });
            Property(t => t.BriefDesc).IsRequired();

            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
