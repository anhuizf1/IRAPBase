using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb031_Ex1")]
    public class RowSet_T1R1 : BaseRowAttrEntity
    {
       
        public int T2LeafID { get; set; }
    }

    public class RowSet_T1R1Map : EntityTypeConfiguration<RowSet_T1R1>
    {

        public RowSet_T1R1Map()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.PartitioningKey, t.EntityID,t.Ordinal});
            Property(t => t.T2LeafID).IsRequired();
     
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
