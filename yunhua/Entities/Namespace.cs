using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 名称实体类
    /// </summary>
     public class NameSpaceEntity : BaseEntity
    {
        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; } = 0;
        /// <summary>
        /// 名称标识
        /// </summary>
        public int NameID { get; set; } = 0;
        /// <summary>
        /// 语言标识
        /// </summary>
        public Int16 LanguageID { get; set; } = 0;
        /// <summary>
        /// 校验和
        /// </summary>
        public int BChecksum { get; set; } = 0;
        /// <summary>
        /// 名称描述
        /// </summary>
        public string NameDescription { get; set; } = "";
        /// <summary>
        /// 索引代码1
        /// </summary>
        public string SearchCode1 { get; set; } = "";
        /// <summary>
        /// 索引代码2
        /// </summary>
        public string SearchCode2 { get; set; } = "";
        /// <summary>
        /// 帮助代码
        /// </summary>
        public string HelpMemoryCode { get; set; } = "";
    }

    /// <summary>
    /// 系统名称实体类
    /// </summary>
    [Table("stb003")]
    public class SysNameSpaceEntity : NameSpaceEntity
    {

    }

    /// <summary>
    /// 业务名称实体类
    /// </summary>
    [Table("stb004")]
    public class BizNameSpaceEntity : NameSpaceEntity
    {

    }

    /// <summary>
    /// 主数据名称实体类
    /// </summary>
    [Table("SysNamespaces")]
    public class SysNameSpaceMDMEntity : NameSpaceEntity
    {

    }
}
