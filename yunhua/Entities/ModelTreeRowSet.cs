using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    //行集属性的模型
    [Table("stb106")]
    public class ModelTreeRowSet : BaseEntity
    {
        public ModelTreeRowSet()
        {
            RSAttrTBLName = "";
            DicingFilter = "";
            ProcOnSave = "";
            ProcOnETL = "";
            ProcOnVersionApply = "";
            LastUpdatedTime = DateTime.Now;
        }
        public Int16 TreeID { get; set; }    //树标识
        public byte RowSetID { get; set; }    // 行集序号
        public int RSAttrNameID { get; set; }    //行集名称标识
        public string RSAttrTBLName { get; set; }    //行集表名
        public string DicingFilter { get; set; }    //过滤串(用途未知)
        public string ProcOnSave { get; set; }    //保存行集时调用的过程
        public string ProcOnETL { get; set; }   // ETL迁移时调用的过程
        public string ProcOnVersionApply { get; set; }   // 版本应用时调用的过程
        public Int16 ETLStatus { get; set; }    //ETL状态
        public int Version { get; set; }    //版本号
        public int RSChecksum { get; set; }    //行校验和
        public DateTime LastUpdatedTime { get; set; }        // 最后一次更新时间
        public bool Protected { get; set; }    //是否保护 = 1 是只读（在主数据管理界面不允许修改
        public bool CommunityIndependent { get; set; }    //是否为社区隔离的
        public int T3LeafID { get; set; }     //功能标识（用途未知）

    }


    public class ModelTreeRowSetMap : EntityTypeConfiguration<ModelTreeRowSet>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelTreeRowSetMap()
        {
            //表定义
            //ToTable("stb031_Ex1");
            HasKey(t => new { t.TreeID, t.RowSetID });
            Property(t => t.RSAttrTBLName).IsRequired();
            // Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
