using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    //这个类计划从 系统表中获取元数据 ，所以是只读的类
    [Table("stb039")]
    public class ModelTreeGeneral:BaseGenAttrEntity
    {
        public ModelTreeGeneral()
        {
            CollationRule = "";
            MimeType = "";
        }
        public string Type { get; set; }
        public Int16 Length { get; set; }
        public byte DataPrecision { get; set; }
        public byte DecimalDigits { get; set; }
        public bool AllowNull { get; set; }
        public bool IsIdentity { get; set; }
        public int IdentitySeed { get; set; }
        public int IdentityInc { get; set; }
        public bool IsRowGuid { get; set; }
        public string CollationRule { get; set; }
        public Int16 FieldWidth { get; set; }
        public string MimeType { get; set; }
    }


    public class ModelTreeGeneralMap : EntityTypeConfiguration<ModelTreeGeneral>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeGeneralMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.PartitioningKey, t.EntityID });
            Property(t => t.MimeType).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
