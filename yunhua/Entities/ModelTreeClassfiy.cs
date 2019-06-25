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
    /// 分类属性定义表
    /// </summary>
   [Table("stb112")]
    public class ModelTreeClassfiyEntity : BaseEntity
    {

        public ModelTreeClassfiyEntity()
        {
            DicingFilter = "";
        }
        /// <summary>
        /// 树标识
        /// </summary>
        public Int16 TreeID { get; set; }
        /// <summary>
        /// 属性序号
        /// </summary>
        public byte AttrIndex { get; set; }
        /// <summary>
        /// 分类属性树
        /// </summary>
        public Int16 AttrTreeID { get; set; }
        /// <summary>
        /// 分类属性名称
        /// </summary>
        public int AttrNameID { get; set; }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string DicingFilter { get; set; }
        /// <summary>
        /// 是否保留历史
        /// </summary>
        public bool SaveChangeHistory { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        public byte NodeDepth { get; set; }
        /// <summary>
        /// 是否必录项
        /// </summary>
        public bool Required { get; set; }
    }

    /// <summary>
    /// 映射类
    /// </summary>
    public class ModelTreeClassfiyEntityMap : EntityTypeConfiguration<ModelTreeClassfiyEntity>
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeClassfiyEntityMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.TreeID, t.AttrIndex });
            Property(t => t.AttrTreeID).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
