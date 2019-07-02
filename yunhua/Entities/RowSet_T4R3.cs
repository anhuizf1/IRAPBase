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
    /// 业务操作第三行集：度量定义
    /// </summary>
    [Table("stb034_Ex3")]
    public class RowSet_T4R3 : BaseRowAttrEntity
    {
        public Int16 TreeID { get; set; }
        public string SrcFieldName { get; set; }
        public int DimNameID { get; set; }
        public bool Filterable { get; set; }
        public bool Traceable { get; set; }
        public long BitCtrl_ShowMode { get; set; }
        public decimal DataShowOrdinal { get; set; }

    }

    public class RowSet_T4R3Map : EntityTypeConfiguration<RowSet_T4R3>
    {
        public RowSet_T4R3Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });
            Property(t => t.DimNameID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
