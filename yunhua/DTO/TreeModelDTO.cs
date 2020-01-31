using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    /// <summary>
    /// 返回Tree清单
    /// </summary>
    public class TreeModelDTO
    {
        /// <summary>
        /// 树标识
        /// </summary>
        public short TreeID { get; set; }
        /// <summary>
        /// 树名称
        /// </summary>
        public string TreeName { get; set; }
        /// <summary>
        /// 树类型
        /// </summary>
        public byte TreeType { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        public int DepthLimit { get; set; }
        /// <summary>
        /// 叶子数量限制
        /// </summary>
        public int LeafLimit { get; set; }
        /// <summary>
        /// 一般属性表名称
        /// </summary>
        public string GenAttrTBLName { get; set; }
        /// <summary>
        /// 结点属性表名称
        /// </summary>
        public string NodeAttrTBName { get; set; }
        /// <summary>
        /// 主标识代码名称
        /// </summary>
        public string PrimaryCodeName { get; set; }
        /// <summary>
        /// 替代代码名称
        /// </summary>
        public string AlternateCodeName { get; set; }
        /// <summary>
        /// 是否为共享树
        /// </summary>
        public bool ShareToAll { get; set; }
        /// <summary>
        /// 主标识代码是否唯一
        /// </summary>
        public bool UniqueEntityCode { get; set; }
        /// <summary>
        /// 结点代码是否唯一
        /// </summary>
        public bool UniqueNodeCode { get; set; }
        /// <summary>
        /// 排序模式1- Code 2- 名称 3-自定义
        /// </summary>
        public byte OrderByMode { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? LastUpdatedTime { get; set; }

       public bool CommunityIndependent { get; set; }

        public bool AutoCodeGenerating { get; set; }

        public byte ExclusiveLevel { get; set; }
    }

    /// <summary>
    /// 分类属性DTO
    /// </summary>
    public class TreeClassifyModelDTO
    {
        public int AttrIndex { get; set; }
        public int AttrTreeID { get; set; }
        public string AttrName { get; set; }
        public bool SaveChangeHistory { get; set; }
    }

    public class TransientDTO
    {

        public long AttrValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public Int16 Scale { get; set; }
        public Double Value
        {
            get { return AttrValue / Math.Pow(10, Scale); }
        }

    }

    public class ClassifyDTO
    {
        public int Ordinal { get; set; }
        public int AttrTreeID { get; set; }
        public int A4LeafID { get; set; }
        public string A4Code { get; set; }
        public string A4AlternateCode { get; set; }
        public string A4NodeName { get; set; }
        public string A4EnglishName { get; set; }
    }

    public class StatusDTO
    {
        public int T5LeafID { get; set; }
        public byte StatusValue { get; set; }
        public int ColorRGBValue { get; set; }
        public long TransitCtrlValue { get; set; }
    }


    public class TreeTransientModelDTO
    {
        public int AttrIndex { get; set; }
        public string AttrName { get; set; }
        public Int16 Scale { get; set; }
        public string UnitOfMeasure { get; set; }
        public bool Protected { get; set; }
    }

    public class TreeStatusModelDTO
    {
        public int AttrIndex { get; set; }

        public string AttrName { get; set; }

        public int T5LeafID { get; set; }

        public bool Protect { get; set; }
    }



    public class TreeStatusModelRowDTO
    {
        // public Int16 TreeID { get; set; }
        public byte Ordinal { get; set; }
        public byte StatusIndex { get; set; }

        public byte Status { get; set; }

        public string StatusName { get; set; }
    }

    /// <summary>
    /// 数据字典：数据表定义
    /// </summary>
    public class IRAPTableColDTO
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisPlayName { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public Int16 Length { get; set; }
        /// <summary>
        ///  精度
        /// </summary>
        public byte DataPrecision { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public byte DecimalDigits { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool AllowNull { get; set; }
        /// <summary>
        /// 是否自增列
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 自增种子
        /// </summary>
        public int IdentitySeed { get; set; }
        /// <summary>
        /// 每次自增步长
        /// </summary>
        public int IdentityInc { get; set; }
        /// <summary>
        /// 是否为GUID列
        /// </summary>
        public bool IsRowGuid { get; set; }
        /// <summary>
        /// 排序规则
        /// </summary>
        public string CollationRule { get; set; }
        /// <summary>
        /// 显示宽度
        /// </summary>
        public Int16 FieldWidth { get; set; }
        /// <summary>
        /// html媒体类型
        /// </summary>
        public string MimeType { get; set; }
    }


    public class RowSetDefineDTO
    {
        public byte AttrIndex { get; set; }
        public string AttrName { get; set; }
        public string RSAttrTBLName { get; set; }
        public string DicingFilter { get; set; }
        public string ProcOnSave { get; set; }
        public string ProcOnETL { get; set; }
        public string ProcOnVersionApply { get; set; }
        public int CurrentVersionNo { get; set; }
        public bool Protected { get; set; }
    }

    /// <summary>
    /// 层次定义
    /// </summary>
    public class TreeLevelDTO
    {
        /// <summary>
        /// 深度：结点深度从1开始递增，叶深度为255
        /// </summary>
        public byte NodeDepth { get; set; }
        /// <summary>
        /// 层次名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// 默认图标ID
        /// </summary>
        public int DefaultIconID { get; set; }
       /// <summary>
       /// 编码规则
       /// </summary>
        public string CodingRule { get; set; }
    }
}
