using IRAPBase.DTO;
using IRAPBase.Entities;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    /// <summary>
    /// 社区对象集维护类
    /// </summary>
    public class IRAPCommunitySet
    {
        private readonly Repository<IRAPCommunityEntity> communtities = null;
        private readonly IDbContext db = null;
        private ILog _log = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPCommunitySet()
        {
            _log = Logger.Get<IRAPCommunitySet>();

            try
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
            } catch (Exception error)
            {
                _log.Error(error.Message, error);
                throw error;
            }

            communtities = new Repository<IRAPCommunityEntity>(db);
        }

        /// <summary>
        /// 根据社区标识和系统登录标识获取登录用户信息
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="loginInfo">输出参数，登录用户信息</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        private IRAPError GetUserInfoWithSysLogID(
            int communityID,
            long sysLogID,
            out LoginEntity loginInfo)
        {
            #region 根据系统登录标识获取登录信息
            IRAPError rlt = new IRAPError();
            loginInfo = null;
            try
            {
                IRAPLog loginSet = new IRAPLog();
                loginInfo = loginSet.GetLogin(communityID, sysLogID);
                if (loginInfo == null)
                {
                    loginInfo = new LoginEntity()
                    {
                        UserCode = "Unknown",
                        LanguageID = 30,
                    };
                }
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"获取登录信息发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"获取登录信息发生异常：{error.Message}";
                }
                _log.Error($"[({rlt.ErrCode}){rlt.ErrText}]");
                rlt.ErrCode = 9999;
            }
            #endregion

            return rlt;
        }

        /// <summary>
        /// 新增一个社区
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="communityCode">社区代码</param>
        /// <param name="dunsCode">企业邓白氏码</param>
        /// <param name="registerYear">社区注册年份</param>
        /// <param name="t1002LeafID">公司叶标识</param>
        /// <param name="t1NodeID">机构入口节点标识</param>
        /// <param name="t291LeafID_Background">背景图片叶标识</param>
        /// <param name="t291LeafID_TopBanner">标题图片叶标识</param>
        /// <param name="loginDivTop">登录框顶部位置</param>
        /// <param name="loginDivLeft">登录框左侧位置</param>
        /// <param name="wcfAddress">WCF应用服务地址（已弃用）</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Add(
            int communityID,
            string communityCode,
            string dunsCode,
            short registerYear,
            int t1002LeafID,
            int t1NodeID,
            int t291LeafID_Background,
            int t291LeafID_TopBanner,
            int loginDivTop,
            int loginDivLeft,
            string wcfAddress,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            if (communityID == 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = "新增的社区标识不能为[0]";
                _log.Error($"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            rlt = 
                GetUserInfoWithSysLogID(
                    communityID, 
                    sysLogID, 
                    out LoginEntity loginInfo);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }
        }
    }
}
