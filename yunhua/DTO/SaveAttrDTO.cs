using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class IRAPTreeAttrDTO
    {
        public int AttrType { get; set; }
        public int AttrIndex { get; set; }
       
        public string AttrCode { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public Int16 Length { get; set; }
        public byte Prec { get; set; }
        public byte Dec { get; set; }
        public bool Protected { get; set; }
        public Int16 AttrScale { get; set; }
        public string UnitOfMeasure { get; set; }
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
    }

    public class SaveAttrInputDTO
    {
        /// <summary>
        /// 属性类型：1- 标识属性 2-目录属性3-检索属性 4-分类属性 5-状态属性 6-瞬态属性 7-一般属性 8-行集（不在这里）
        /// </summary>
        public int AttrType { get; set; }

        /// <summary>
        /// 序号 指定AttrType里，连续且唯一 
        /// </summary>
        public int AttrIndex { get; set; }

        /// <summary>
        /// 对应数据库字段名
        /// </summary>
        public string AttrCode { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 要保存的值
        /// </summary>
        public string AttrValue { get; set; }
    }


}
