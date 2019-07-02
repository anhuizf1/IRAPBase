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
    /// 业务操作的一般属性
    /// </summary>
    [Table("stb034")]
    public class GenAttr_T4 : BaseGenAttrEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GenAttr_T4()
        {
            AuxTranTBLName = "";
            OLTPTempFactTBLName = "";
            OLTPFixedFactTBLName = "";
            TFPartitionPolicy = "";
            FFPartitionPolicy = "";
            AFPartitionPolicy = "";
            RFPartitionPolicy = "";
            ProcOnTranCheck = "";
            ProcOnTranUnCheck = "";
            ProcOnTranCancel = "";
            ProcOnFactSave = "";
            ProcOnFactDelete = "";
            ProcOnFactFix = "";
            ETLProcedureName = "";
            ETLPrimarySourceTable = "";
        }
        public long PartitioningKey { get; set; }
        public int EntityID { get; set; }
        public Int16 TreeCorrID { get; set; }
        public int WFInstanceNameID { get; set; }
        public int LinkedFactNameID { get; set; }
        public string AuxTranTBLName { get; set; }
        public string OLTPTempFactTBLName { get; set; }
        public string OLTPFixedFactTBLName { get; set; }
        public string TFPartitionPolicy { get; set; }
        public string FFPartitionPolicy { get; set; }
        public string AFPartitionPolicy { get; set; }
        public string RFPartitionPolicy { get; set; }
        public string ProcOnTranCheck { get; set; }
        public string ProcOnTranUnCheck { get; set; }
        public string ProcOnTranCancel { get; set; }
        public string ProcOnFactSave { get; set; }
        public string ProcOnFactDelete { get; set; }
        public string ProcOnFactFix { get; set; }
        public string ETLProcedureName { get; set; }
        public string ETLPrimarySourceTable { get; set; }
        public int DataSrcLinkID { get; set; }
    }



    public class GenAttr_T4Map : EntityTypeConfiguration<GenAttr_T4>
    {
        public GenAttr_T4Map()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.EntityID });
            Property(t => t.OLTPTempFactTBLName).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
