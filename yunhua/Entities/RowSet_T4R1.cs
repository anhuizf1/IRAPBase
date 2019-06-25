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
    /// 业务操作第一行集：操作类型行集
    /// </summary>
    [Table("stb034_Ex1")]
    public class RowSet_T4R1:BaseRowAttrEntity
    {
       /// <summary>
       /// 构造函数
       /// </summary>
        public RowSet_T4R1()
        {
            AuxFactTBLName = "";
            ComplementaryRule = "";
            StateExclCtrlStr = "";
            AuthorizingCondition = "";
        }
        public Int16 OpType { get; set; }
        public int T18LeafID { get; set; }
        public string AuxFactTBLName { get; set; }
        public string ComplementaryRule { get; set; }
        public string StateExclCtrlStr { get; set; }
        public bool EntityCreating { get; set; }
        public bool BusinessDateIsValid { get; set; }
        public bool IsValid { get; set; }
        public string AuthorizingCondition { get; set; }

    }


    public class RowSet_T4R1Map : EntityTypeConfiguration<RowSet_T4R1>
    {
        public RowSet_T4R1Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID ,t.Ordinal});
            Property(t => t.OpType).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
