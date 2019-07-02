using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb035_Ex1")]
    public class ModelTreeStatusList : BaseRowAttrEntity
    {
        public ModelTreeStatusList()
        {
            VersionLE = (int)(Math.Pow(2, 31) - 1);
            Parameters = "";
        }
       
        public int StatusNameID { get; set; }
        public byte Status { get; set; }
        public int ColorRGBValue { get; set; }
        public string Parameters { get; set; }
        public long TransitCtrlValue { get; set; }
    }

    public class ModelTreeStatusListMap : EntityTypeConfiguration<ModelTreeStatusList>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeStatusListMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.EntityID, t.Ordinal,t.VersionLE });
            Property(t => t.Status).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }

}
