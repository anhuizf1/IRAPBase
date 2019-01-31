using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class LoginEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//不自动增长
        public long SysLogID { get; set; }
        public string Access_Token { get; set; }
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
