using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 树层次定义实体对象
    /// </summary>
    [Table("stb165")]
    public class TreeLevelEntity:BaseEntity
    {
        public TreeLevelEntity()
        {
            CodingRule = "";
        }
        public long PartitioningKey { get; set; }
        public Int16 TreeID { get; set; }
        public byte NodeDepth { get; set; }
        public int LevelAliasNameID { get; set; }
        public int DefaultIconID { get; set; }

        public string CodingRule { get; set; }
    }

    public class TreeLevelEntityMap : EntityTypeConfiguration<TreeLevelEntity>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TreeLevelEntityMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.TreeID,t.NodeDepth});
            Property(t => t.LevelAliasNameID).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
