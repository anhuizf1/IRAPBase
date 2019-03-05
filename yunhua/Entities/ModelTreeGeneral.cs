using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    //这个类计划从 系统表中获取元数据 ，所以是只读的类
    //数据字典
    public class ModelTreeGeneral
    {
        public int EntityID { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public int DataPrecision { get; set; }
        public int DecimalDigits { get; set; }
        public bool AllowNull { get; set; }
        public bool IsIdentity { get; set; }
        public int IdentitySeed { get; set; }
        public int IdentityInc { get; set; }
        public bool IsRowGuid { get; set; }
        public string CollationRule { get; set; }
        public int FieldWidth { get; set; }
        public string MimeType { get; set; }
    }
}
