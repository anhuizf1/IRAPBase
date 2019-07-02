using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
namespace IRAPBase
{
    /// <summary>
    /// IRAP报错模版
    /// </summary>
    public class IRAPErrorSet
    {
        private IRAPErrorSet _instanceID = null;
        private IDbContext _db = null;
        /// <summary>
        /// 单粒类
        /// </summary>
        public IRAPErrorSet InstanceID
        {
            get
            {
                if (_instanceID == null)
                {
                    _instanceID = new IRAPErrorSet();
                }
                return _instanceID;
            }
        }

        public IRAPErrorSet()
        {
            _db = DBContextFactory.Instance.CreateContext("IRAPContext");
        }

        /// <summary>
        /// 获取错误内容
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string ErrTextDesc(int languageID, int errCode, params string[] msg)
        {
            var entity = _db.Set<ErrorMsgTemplateEntity>().FirstOrDefault(c => c.LanguageID == languageID && c.ErrorID == errCode);
            if (entity == null)
            {
                throw new Exception($"错误消息模版不存在！ErrorID={errCode}");
            }
            int i = 0;
            string rtnMsg = entity.Description;
            foreach(string item in msg)
            {
                i++;
                rtnMsg = rtnMsg.Replace($"%{i}!", item);
            }
            return rtnMsg;
        }



    }
}
