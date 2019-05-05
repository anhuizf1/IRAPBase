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
    /// 自定义界面属性维护
    /// </summary>
    [Table("stb173")]
    public class IRAPTreeAttrEntity : BaseEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPTreeAttrEntity()
        {
            UnitOfMeasure = "";
            TargetField = "";
            AgencyNodeList = "";
            RoleNodeList = "";
            LabelColor = "";
            LabelFont = "";
            LabelFontSize = "";
            LabelAlignment = "";
            ControlType = "";
            ControlColor = "";
            ControlFont = "";
            ControlFontSize = "";
            ControlAlignment = "";
            MoreCtrlAttrs = "";
        }
        public long PartitioningKey { get; set; }
        public Int16 LanguageID { get; set; }
        public byte NodeType { get; set; }
        public Int16 EntryTreeID { get; set; }
        public int TreeID { get; set; }
        public int AttrType { get; set; }
        public int AttrIndex { get; set; }
        public byte RSColNo { get; set; }
        public byte RefOrdinal { get; set; }
        public string AttrCode { get; set; }
        public string AttrName { get; set; }
        public string Type { get; set; }
        public Int16 Length { get; set; }
        public byte Prec { get; set; }
        public byte Dec { get; set; }
        public bool Protected { get; set; }
        public Int16 AttrScale { get; set; }
        public string UnitOfMeasure { get; set; }
        public string TargetField { get; set; }
        public byte AltCodeIndex { get; set; }
        public string AgencyNodeList { get; set; }
        public string RoleNodeList { get; set; }
        public bool Required { get; set; }
        public Int16 LabelLength { get; set; }
        public string LabelColor { get; set; }
        public string LabelFont { get; set; }
        public string LabelFontSize { get; set; }
        public string LabelAlignment { get; set; }
        public string ControlType { get; set; }
        public Int16 ControlWidth { get; set; }
        public string ControlColor { get; set; }
        public string ControlFont { get; set; }
        public string ControlFontSize { get; set; }
        public string ControlAlignment { get; set; }
        public string MoreCtrlAttrs { get; set; }
    }


    public class IRAPTreeAttrEntityMap : EntityTypeConfiguration<IRAPTreeAttrEntity>
    {

        public IRAPTreeAttrEntityMap()
        {
            //表定义
            // ToTable("stb006");
            HasKey(t => new { t.PartitioningKey, t.TreeID, t.AttrType, t.AttrIndex, t.EntryTreeID });
            // Property(t => t.TreeID).IsRequired();
            Property(t => t.AttrCode).IsRequired();
            Property(t => t.AttrName).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
