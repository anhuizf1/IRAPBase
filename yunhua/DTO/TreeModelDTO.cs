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
        public Int16 TreeID { get; set; }
        public string TreeName { get; set; }
        public string TreeType { get; set; }
        public int DepthLimit { get; set; }
        public int LeafLimit { get; set; }
        public string GenAttrTBLName { get; set; }
        public string NodeAttrTBName { get; set; }
        public string PrimaryCodeName { get; set; }
    }
}
