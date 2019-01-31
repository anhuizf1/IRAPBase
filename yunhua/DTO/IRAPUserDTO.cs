using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{

    public class IRAPError
    {
        public IRAPError()
        {

        }
        public IRAPError(int errCode, string errText)
        {
            this.ErrCode = errCode;
            this.ErrText = errText;
        }
        public int ErrCode { get; set; }
        public string ErrText { get; set; }
    }
    public class BackLeafSetDTO
    {
        public int ErrCode { get; set; }
        public string ErrText { get; set; }

        public List<LeafDTO> Rows { get; set; }
    }

    public class LeafDTO
    {

        public int Leaf { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class BackLoginInfo
    {
        public string UserName { get; set; }
        // OUTPUT--用户姓名
        public string NickName { get; set; }
        //  OUTPUT--用户昵称
        public long SysLogID { get; set; }
        //OUTPUT--系统登录标识

        public string access_token { get; set; }
        public int LanguageID { get; set; }
        // OUTPUT--语言标识
        public string OPhoneNo { get; set; }
        //OUTPUT--办公电话
        public string HPhoneNo { get; set; }
        // OUTPUT--住宅电话
        public string MPhoneNo { get; set; }  //OUTPUT--移动电话
        //public int AgencyID { get; set; }  // OUTPUT--机构标识
        public string AgencyName { get; set; }//   OUTPUT--机构名称
        public string HostName { get; set; }
        public int ErrCode { get; set; }
        // OUTPUT--错误编号
        public string ErrText { get; set; }
        //OUTPUT--错误文本

    }
}
