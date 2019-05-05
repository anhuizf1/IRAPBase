using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{

    [Table("stb109")]
    public class ModelTreeTransient:BaseEntity
    {
        public Int16 TreeID { get; set; }
        public byte StatIndex { get; set; }
        public int StatNameID { get; set; }
        public string UnitOfMeasure { get; set; }
        public Int16 Scale { get; set; }
        public bool Protected { get; set; }
        //public string AgencyNodeList { get; set; }
       // public string RoleNodeList { get; set; }
    }
    public class ModelTreeTransientMap : EntityTypeConfiguration<ModelTreeTransient>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeTransientMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.TreeID, t.StatIndex });
            Property(t => t.UnitOfMeasure).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
