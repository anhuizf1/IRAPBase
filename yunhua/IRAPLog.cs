using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPLog
    {
        private IQueryable<LoginEntity> _loginLog = null;

        public IQueryable<LoginEntity> LoginLog { get { return _loginLog; } }
        public IRAPLog()
        {
            _loginLog = new Repository<LoginEntity>("IRAPContext").Table;
        }

        /// <summary>
        /// 获取登录日志
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="sysLogID">登录标识</param>
        /// <returns></returns>
        public LoginEntity GetLogin(
            int communityID,
            long sysLogID)
        {
            return
                _loginLog
                    .Where(
                        r => r.PartitioningKey == communityID * 10000 &&
                        r.SysLogID == sysLogID)
                    .FirstOrDefault();
        }

        public LoginEntity GetLogIDByToken(string access_token )
        {
            try
            {
                LoginEntity e = _loginLog.FirstOrDefault(r => r.Access_Token == access_token);

                return e;
            }
            catch(Exception err)
            {
                return null;
            }
            
        }
        public int GetCommunityID (string access_token)
        {
            LoginEntity e = GetLogIDByToken(access_token);
            if (e != null)
            {
                return (int)e.PartitioningKey / 10000;
            }
            else
            {
                return 0;
            }
        }
    }
}
