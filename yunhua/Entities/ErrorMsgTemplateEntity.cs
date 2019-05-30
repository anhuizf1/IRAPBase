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
   /// 错误消息模版
   /// </summary>
    [Table("stb154")]
    public class ErrorMsgTemplateEntity : BaseEntity
    {

        /// <summary>
        /// 分区键
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public Int16 LanguageID { get; set; }
        /// <summary>
        /// 验证级别
        /// </summary>
        public byte Severity { get; set; }
        /// <summary>
        /// 是否发生到消息总线
        /// </summary>
        public bool SendToMessageBus { get; set; }
        /// <summary>
        /// 错误消息目标内容
        /// </summary>
        public string Description { get; set; }
    }


    public class ErrorMsgTemplateEntityMap : EntityTypeConfiguration<ErrorMsgTemplateEntity>
    {
        public ErrorMsgTemplateEntityMap()
        {
            //表定义
            //ToTable("stb031");
            HasKey(t => new { t.PartitioningKey, t.ErrorID,t.LanguageID });
            Property(t => t.Description).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
