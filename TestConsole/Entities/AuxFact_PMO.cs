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
    [Table("AuxFact_PMO")]
    public class AuxFact_PMO: BaseAuxFact
    {
        public string Remark { get; set; }
    }

    public class AuxFact_PMOMap : EntityTypeConfiguration<AuxFact_PMO>
    {
        public AuxFact_PMOMap()
        {
            //表定义
            // ToTable("GenAttr_T102");
            HasKey(t => new { t.PartitioningKey, t.FactID });
            //设置实体属性 为空检查
            //Property(t => t.StatusValue).IsRequired();
            //Property(t => t.T5LeafID).IsRequired();
            //Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
