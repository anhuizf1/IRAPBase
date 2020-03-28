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
    /// 菜单缓冲区标识类
    /// </summary>
    public class IRAPMenuCacheEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int MenuCacheID { get; set; }
        public int SystemID { get; set; }
        public int AgencyLeaf { get; set; }
        public int RoleLeaf { get; set; }
        public int StationGroupID { get; set; }
        public short LanguageID { get; set; }
        public byte? ProgLanguageID { get; set; }
        public Int16 MenuStyle { get; set; }
    }

    /// <summary>
    /// 菜单缓冲区标识实体类和数据库表的映射关系类
    /// </summary>
    public class IRAPMenuCacheMap :
        EntityTypeConfiguration<IRAPMenuCacheEntity>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPMenuCacheMap()
        {
            ToTable("stb158");
            HasKey(t => new { t.PartitioningKey, t.MenuCacheID });

        }
    }
}
