using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb034_Ex4")]
    public class RowSet_T4R4 : BaseRowAttrEntity
    {
        public RowSet_T4R4()
        {
            RSFactTBLName = "";
            ProcOnRSFactSave = "";
            ProcOnRSFactAppend = "";
            ProcOnRSFactDelete = "";
        }
        public int RSFactNameID { get; set; }
        public string RSFactTBLName { get; set; }
        public string ProcOnRSFactSave { get; set; }
        public string ProcOnRSFactAppend { get; set; }
        public string ProcOnRSFactDelete { get; set; }
    }

    public class RowSet_T4R4Map : EntityTypeConfiguration<RowSet_T4R4>
    {
        public RowSet_T4R4Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID, t.Ordinal });
            Property(t => t.RSFactNameID).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
