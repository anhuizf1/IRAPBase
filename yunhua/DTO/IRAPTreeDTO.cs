using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class NewTreeNodeDTO
    {
        public int NewEntityID { get; set; }
        public long PartitioningKey { get; set; }
        public int NewNodeID { get; set; }

        public int ErrCode { get; set; }

        public string ErrText { get; set; }
    }

    public class TreeViewDTO
    {
        public int NodeID { get; set; }          //  --结点标识
        public byte TreeViewType { get; set; }    // --树视图类型
        public byte NodeType { get; set; }        //  --结点类型
        public string NodeCode { get; set; }      //   --结点代码
        public string AlternateCode { get; set; }
        public string NodeName { get; set; }       //  --结点名称
        public int Parent { get; set; }            //  --父结点标识
        public byte NodeDepth { get; set; }         // --结点深度
        public int CSTRoot { get; set; }            //--权限控制点
        public float UDFOrdinal { get; set; }       // --自定义遍历序
        public byte NodeStatus { get; set; }         // --结点状态
        public byte Accessibility { get; set; }      // --可访问性  0=不可选  1=可单选  2=可复选
        public byte SelectStatus { get; set; }       // --选中状态  0=未选中  1=已选中
        public string SearchCode1 { get; set; }      //--第一检索码
        public string SearchCode2 { get; set; }      //--第二检索码
        public string HelpMemoryCode { get; set; }   //--助记码
        public string IconFile { get; set; }        // --图标文件
        public byte[] IconImage { get; set; }       // --图标图像
    }


    public class TreeClassifyDTO
    {
        public int Ordinal { get; set; }
        //  public int TreeID { get; set; }
        public int DimLeaf { get; set; }

    }

    /// <summary>
    /// 可访问的权限控制点清单DTO
    /// </summary>
    public class AccessibleCSTDTO
    {
        public long PartitioningKey { get; set; }           //   --分区键
        public int CSTRoot { get; set; }                          // --权限控制点
        public bool Accessible { get; set; }                     //是否权限
    }


    public class TreeDimDTO
    {
        public byte Index { get; set; }
        public Int16 TreeID { get; set; }
    }


    /// <summary>
    /// 分类属性行集（与stb063）
    /// </summary>
    public class TreeClassifyRowDTO {

        public int TreeID { get; set; }
        public int LeafID { get; set; }
        public int Leaf01 { get; set; }
        public int Leaf02 { get; set; }
        public int Leaf03 { get; set; }
        public int Leaf04 { get; set; }
        public int Leaf05 { get; set; }
        public int Leaf06 { get; set; }
        public int Leaf07 { get; set; }
        public int Leaf08 { get; set; }
        public int Leaf09 { get; set; }
        public int Leaf10 { get; set; }
        public int Leaf11 { get; set; }
        public int Leaf12 { get; set; }

        public string  Code01 { get; set; }
        public string Code02 { get; set; }
        public string Code03 { get; set; }
        public string Code04 { get; set; }
        public string Code05 { get; set; }
        public string Code06 { get; set; }
        public string Code07 { get; set; }
        public string Code08 { get; set; }
        public string Code09 { get; set; }
        public string Code10 { get; set; }
        public string Code11 { get; set; }
        public string Code12 { get; set; }
    }

}
