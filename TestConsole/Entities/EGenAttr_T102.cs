using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Entities
{
    /// <summary>
    /// 产品实体
    /// </summary>
    public class EGenAttr_T102 : BaseGenAttrEntity
    {
        /// <summary>
        /// 唯一标识
        /// </summary>


        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 实体ID
        /// </summary>


        /// <summary>
        /// 规格型号
        /// </summary>
        public string SpecificationType { get; set; }

        /// <summary>
        /// 产品属性
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 安全库存
        /// </summary>
        public string SafetyStock { get; set; }

        /// <summary>
        /// 种类
        /// </summary>
        public string ClassType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }


    public class EGenAttr_T102Map : EntityTypeConfiguration<EGenAttr_T102>
    {
        public EGenAttr_T102Map()
        {
            //表定义
            ToTable("GenAttr_T102");
            HasKey(t => new { t.PartitioningKey, t.EntityID });

            //设置实体属性 为空检查
            //Property(t => t.StatusValue).IsRequired();
            //Property(t => t.T5LeafID).IsRequired();
            //Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
