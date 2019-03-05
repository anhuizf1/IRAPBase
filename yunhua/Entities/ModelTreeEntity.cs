using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    //树的模型
    [Table("stb051")]
    public class ModelTreeEntity:BaseEntity
    {
        public Int16 TreeID { get; set; }
        public int NameID { get; set; }
        public int LeafLimit { get; set; }
        public int DepthLimit { get; set; }
        public byte TreeType { get; set; }
        public string EntityAttrTBName { get; set; }
        public string NodeAttrTBName { get; set; }
        public int EntityCodeNameID { get; set; }
        public int AlternateCodeNameID { get; set; }
        public bool SaveRenameHistory { get; set; }
        public bool CommunityIndependent { get; set; }
        public bool ShareToAll { get; set; }
        public int NodeAttrEditFormID { get; set; }
        public int EntityAttrEditFormID { get; set; }
        public int NodeAttrEditHTTPURLID { get; set; }
        public int EntityAttrEditHTTPURLID { get; set; }
        public byte ETLStatus { get; set; }
        public bool RuggedHierarchy { get; set; }
        public bool UniqueEntityCode { get; set; }
        public bool UniqueNodeCode { get; set; }
        public bool AutoCodeGenerating { get; set; }
        public int OrderByMode { get; set; }
        public bool UsingTreeViewCache { get; set; }
        public bool ExclusiveLevel { get; set; }
        public Int16 DefaultLanguageID { get; set; }
        public int HistoricLeafIconID { get; set; }
        public string ProcOnNodeChange { get; set; }
        public string ProcOnAttrSave { get; set; }
        public string ProcOnETL { get; set; }
        public string StatisticRule { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int ClassifyingAttrNameID { get; set; }
        public int StateAttrNameID { get; set; }
        public int InstantaneousAttrNameID { get; set; }
        public int GenAttrNameID { get; set; }
        public int RowSetAttrsNameID { get; set; }
        public string DefaultIDNavigationSetting { get; set; }
        public string DefaultAttrMgmtSetting { get; set; }
        public string DefaultTVCtrlParameters { get; set; }

        //关联名称空间表

    }
 
}
