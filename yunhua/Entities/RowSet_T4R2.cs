using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 业务操作第二行集：维度定义
    /// </summary>
    [Table("stb034_Ex2")]
    public class RowSet_T4R2 : BaseRowAttrEntity
    {
        public RowSet_T4R2()
        {
            UnitOfMeasure = "";
        }
        public int NameID { get; set; }
        public Int16 Scale { get; set; }
        public string UnitOfMeasure { get; set; }
    }


    public class RowSet_T4R2Map : EntityTypeConfiguration<RowSet_T4R2>
    {
        public RowSet_T4R2Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });
            Property(t => t.Scale).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
