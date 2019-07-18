using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    /// <summary>
    /// 对应表 stb009
    /// </summary>
    public class LoginEntity : BaseEntity
    {
        /// <summary>
        /// 分区标识(社区标识*10000)
        /// </summary>
        public long PartitioningKey { get; set; }
        /// <summary>
        /// 登录标识(不自动增长)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SysLogID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 渠道标识(网关 stb_Channels中的ClientID)
        /// </summary>
        public string ClientID { get; set; }
        public byte LoginMode { get; set; }
        public string UserCode { get; set; }
        public int AgencyLeaf { get; set; }
        public int RoleLeaf { get; set; }
        public string MPhoneNo { get; set; }
        public string T1001Code { get; set; }
        public string T1002Code { get; set; }
        public string StationID { get; set; }
        public string IPAddress { get; set; }
        public Int16 LanguageID { get; set; }
        public Int16 TimeZone { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string DBName { get; set; }
        public Int16 DBProcessID { get; set; }
        public string UserDiary { get; set; }
        public byte Status { get; set; }

        public LoginEntity()
        {
            TimeZone = 8;
            LoginTime = DateTime.Now;
            DBName = "IRAP";
            T1001Code = "";
            T1002Code = "";
            UserDiary = "已登录";
            DBProcessID = 1;
        }
    }
}
