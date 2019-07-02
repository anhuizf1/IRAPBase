using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb101")]
    public class ModelTreeCorrEntity:BaseEntity
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public Int16 TreeCorrID { get; set; }
        public string CorrAttrTBLName { get; set; }
        public string CorrAttrTBLNameEx { get; set; }
        public Int16 TreeID01 { get; set; }
        public Int16 TreeID02 { get; set; }
        public Int16 TreeID03 { get; set; }
        public Int16 TreeID04 { get; set; }
        public Int16 TreeID05 { get; set; }
        public Int16 TreeID06 { get; set; }
        public Int16 TreeID07 { get; set; }
        public Int16 TreeID08 { get; set; }
        public Int16 TreeID09 { get; set; }
        public Int16 TreeID10 { get; set; }
        public Int16 TreeID11 { get; set; }
        public Int16 TreeID12 { get; set; }
    }


    public class ModelTreeCorrEntityMap : EntityTypeConfiguration<ModelTreeCorrEntity>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeCorrEntityMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t =>  t.TreeCorrID);
            Property(t => t.CorrAttrTBLName).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
