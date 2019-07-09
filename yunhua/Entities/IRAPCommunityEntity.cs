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
    /// 社区基本信息类
    /// </summary>
    public class IRAPCommunityEntity : BaseEntity
    {
        /// <summary>
        /// 社区标识
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CommunityID { get; set; }
        /// <summary>
        /// 社区代码
        /// </summary>
        public string CommunityCode { get; set; }
        /// <summary>
        /// 邓白氏码
        /// </summary>
        public string DunsCode { get; set; }
        /// <summary>
        /// 注册年份
        /// </summary>
        public byte RegistYear { get; set; }
        public int T1002LeafID { get; set; }
        public int T1NodeID { get; set; }
        public int T291LeafID_Background { get; set; }
        /// <summary>
        /// 登录界面背景图片
        /// </summary>
        public byte[] LoginBackground { get; set; }
        public int T291LeafID_TopBanner { get; set; }
        /// <summary>
        /// 标题图片
        /// </summary>
        public byte[] TopBanner { get; set; }
        /// <summary>
        /// 登录信息框顶部位置
        /// </summary>
        public int LoginDivTop { get; set; }
        /// <summary>
        /// 登录信息框左侧位置
        /// </summary>
        public int LoginDivLeft { get; set; }
        /// <summary>
        /// 应用服务地址
        /// </summary>
        public string WCFAddress { get; set; }
    }

    /// <summary>
    /// 社区基本信息实体类和数据库表的映射关系类
    /// </summary>
    public class IRAPCommunityMap :
        EntityTypeConfiguration<IRAPCommunityEntity>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPCommunityMap()
        {
            ToTable("stb013");
            HasKey(t => new { t.CommunityID });

            Property(t => t.CommunityID).IsRequired();
            Property(t => t.CommunityCode).IsRequired();
            Property(t => t.DunsCode).IsRequired();
            Property(t => t.RegistYear).IsRequired();
            Property(t => t.T1002LeafID).IsRequired();
            Property(t => t.T1NodeID).IsRequired();
            Property(t => t.T291LeafID_Background).IsRequired();
            Property(t => t.T291LeafID_TopBanner).IsRequired();
            Property(t => t.LoginDivTop).IsRequired();
            Property(t => t.LoginDivLeft).IsRequired();
            Property(t => t.WCFAddress).IsRequired();
        }
    }
}
