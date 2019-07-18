using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    /// <summary>
    /// 社区对象类
    /// </summary>
    public class IRAPCommunityDTO
    {
        /// <summary>
        /// 社区标识
        /// </summary>
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
        /// <summary>
        /// 单位叶标识
        /// </summary>
        public int T1002LeafID { get; set; }
        /// <summary>
        /// 机构树入口节点标识
        /// </summary>
        public int T1NodeID { get; set; }
        /// <summary>
        /// 背景图片标识
        /// </summary>
        public int T291LeafID_Background { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public byte[] Background { get; set; }
        /// <summary>
        /// 标题栏图片标识
        /// </summary>
        public int T291LeafID_TopBanner { get; set; }
        /// <summary>
        /// 标题栏图片
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
}
