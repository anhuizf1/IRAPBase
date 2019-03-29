using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{

    /// <summary>
    /// IRAP系统通用错误类
    /// </summary>
    public class IRAPError
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPError()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errCode">错误代码</param>
        /// <param name="errText">错误消息</param>
        public IRAPError(int errCode, string errText)
        {
            this.ErrCode = errCode;
            this.ErrText = errText;
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
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


    /// <summary>
    /// 用户登录返回的DTO
    /// </summary>
    public class BackLoginInfo
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 系统登录标识
        /// </summary>
        public long SysLogID { get; set; }

        /// <summary>
        /// 系统登录令牌（36位GUID）
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 登录语言30-中文 28-繁體中文 0-American English
        /// </summary>
        public int LanguageID { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        public string OPhoneNo { get; set; }

        /// <summary>
        /// 住宅电话
        /// </summary>
        public string HPhoneNo { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string MPhoneNo { get; set; }
        //public int AgencyID { get; set; }   
        /// <summary>
        /// 机构名称
        /// </summary>
        public string AgencyName { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrText { get; set; }

    }
}
