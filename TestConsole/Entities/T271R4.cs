using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Entities
{

    [Table("RSAttr_T271R4")]
    public class ERS_T271R4 : BaseRowAttrEntity
    {
        public int T199LeafID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }

    public class ERS_T271R4Map : EntityTypeConfiguration<ERS_T271R4>
    {
        public ERS_T271R4Map()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });
            Property(t => t.Code).IsRequired();
            Property(t => t.Name).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
