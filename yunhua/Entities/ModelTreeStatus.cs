using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb113")]
    public class ModelTreeStatus : BaseEntity
    {
        public Int16 TreeID { get; set; }
        public byte StateIndex { get; set; }
        public int T5LeafID { get; set; }
        public byte LSBIndex { get; set; }
        public byte MSBIndex { get; set; }
        public bool Protected { get; set; }
 
    }

    public class ModelTreeStatusMap : EntityTypeConfiguration<ModelTreeStatus>
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeStatusMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.TreeID, t.StateIndex });
            Property(t => t.T5LeafID).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
