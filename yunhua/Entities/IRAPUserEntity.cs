using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class IRAPUserEntity:BaseEntity
    {
        public long PartitioningKey { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserEnglishName { get; set; }
        public byte[] EncryptedPWD { get; set; }
        public string AgencyNodeList { get; set; }
        public string RoleNodeList { get; set; }
        public Int16 LanguageID { get; set; }
        public Int16 TimeZone { get; set; }
        public byte Status { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public byte Gender { get; set; }
        public Int16 MsgType { get; set; }
        public string PersonType { get; set; }
        public string IDType { get; set; }
        public string PID { get; set; }
        public DateTime? BirthDate { get; set; }
        public string OPhoneNo { get; set; }
        public string HPhoneNo { get; set; }
        public string MPhoneNo { get; set; }
        public string PagerNo { get; set; }
        public string MailBoxNo { get; set; }
        public string EmailAddr { get; set; }
        public string InstantMessageID { get; set; }
        public string WeChatID { get; set; }
        public string WearableDeviceID { get; set; }
        public byte[] SmallHeadPic { get; set; }
        public DateTime RegistedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public byte VerifyType { get; set; }
        public string VerifyCode { get; set; }
        public DateTime? VerifyInvalidTime { get; set; }
        public Int16 VerifySuccess { get; set; }
        public string CardNo { get; set; }
        public string JobCode { get; set; }
        public string JobRankCode { get; set; }
        public string JobGradeCode { get; set; }
        public bool BeingAtWork { get; set; }
        public byte EventRespondingPriority { get; set; }
        public long LinkedToEventID { get; set; }
        public string OtherSystemUserCode { get; set; }
        public string OtherSystemUserPWD { get; set; }
        public IRAPUserEntity()
        {
            UserEnglishName = "admi";
            AgencyNodeList = "-1";
            RoleNodeList = "-2";
            LanguageID = 30;
            TimeZone = 8;
            Status = 1;
            Question = "问题";
            Answer = "问题的答案";
            Gender = 1;
            MsgType = 1;
            PersonType = "";
            IDType = "1";
            PID = "342224198110210817";
            BirthDate = new DateTime(1982, 6, 12);
            OPhoneNo = "";
            HPhoneNo = "";
            MPhoneNo = "13913911904";
            PagerNo = "pagerno";
            MailBoxNo = "";
            EmailAddr = "star3@163.com";
            InstantMessageID = "";
            WeChatID = "";
            WearableDeviceID = "";
            RegistedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;

            VerifyType = 1;
            VerifyCode = "";
            VerifySuccess = 0;
            CardNo = "";
            BeingAtWork = false;
            EventRespondingPriority = 1;
            LinkedToEventID = 0;
            OtherSystemUserCode = "";
            OtherSystemUserPWD = "";
            EncryptedPWD = new byte[8];
            SmallHeadPic = new byte[8];
            LeaveTime = DateTime.Now;
            VerifyInvalidTime = DateTime.Now;
        }
    }
}

