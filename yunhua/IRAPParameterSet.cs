using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IRAPBase.Entities;
using IRAPBase.DTO;
using IRAPBase.Enums;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace IRAPBase
{
    /// <summary>
    /// IRAP 参数集维护类
    /// </summary>
    public class IRAPParameterSet
    {
        private Repository<IRAPParameterEntity> irapParams = null;
        private Repository<SysNameSpaceEntity> names = null;
        private IDbContext db = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPParameterSet()
        {
            try
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
            }
            catch (Exception error) { throw error; }

            irapParams = new Repository<IRAPParameterEntity>(db);
            names = new Repository<SysNameSpaceEntity>(db);
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
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                rlt.ErrCode = 9999;
            }
            #endregion

            return rlt;
        }

        /// <summary>
        /// 获取 IRAP 全局参数表
        /// </summary>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetGlobal(long sysLogID)
        {
            return GetByCommunityID(0, sysLogID);
        }

        /// <summary>
        /// 获取社区相关的参数表
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByCommunityID(
            int communityID,
            long sysLogID)
        {
            IQueryable<IRAPParameterEntity> dtParams = irapParams.Table;
            IQueryable<SysNameSpaceEntity> dtNames = names.Table;

            IRAPError rlt =
                GetUserInfoWithSysLogID(communityID, sysLogID, out LoginEntity userInfo);
            short languageID = rlt.ErrCode == 0 ? userInfo.LanguageID : (short)30;

            return
                (from s in
                    dtParams
                        .Where(p => p.PartitioningKey == communityID * 10000)
                 join l in
                     dtNames
                         .Where(
                             t => t.LanguageID == languageID &&
                                (/*t.PartitioningKey == communityID * 10000 ||
                                 */t.PartitioningKey == 0)) // 命名空间和社区无关
                     on s.ParameterNameID equals l.NameID
                 orderby s.ParameterID
                 select new IRAPParameterDTO()
                 {
                     PartitioningKey = s.PartitioningKey,
                     ParameterID = s.ParameterID,
                     ParameterNameID = s.ParameterNameID,
                     ParameterName = l.NameDescription,
                     ParameterValue = s.ParameterValue,
                     ParameterValueStr = s.ParameterValueStr,
                     UpdatedBy = s.UpdatedBy,
                     TimeUpdated = s.TimeUpdated,
                 })
                    .ToList();
        }

        /// <summary>
        /// 获取指定参数 ID 列表的参数表
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="ids">参数 ID 列表</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByParamID(
            int communityID,
            int[] ids,
            long sysLogID)
        {
            IRAPError rlt =
                GetUserInfoWithSysLogID(communityID, sysLogID, out LoginEntity userInfo);
            short languageID = rlt.ErrCode == 0 ? userInfo.LanguageID : (short)30;

            return
                (from s in
                    irapParams
                        .Table
                        .Where(
                            p => ids.Contains(p.ParameterID) &&
                            p.PartitioningKey == communityID * 10000)
                 join l in
                    names
                        .Table
                        .Where(
                            t => t.LanguageID == languageID &&
                                (/*t.PartitioningKey == communityID * 10000 ||
                                 */t.PartitioningKey == 0))  // 命名空间和社区无关
                    on s.ParameterNameID equals l.NameID
                 orderby s.ParameterID
                 select new IRAPParameterDTO()
                 {
                     PartitioningKey = s.PartitioningKey,
                     ParameterID = s.ParameterID,
                     ParameterNameID = s.ParameterNameID,
                     ParameterName = l.NameDescription,
                     ParameterValue = s.ParameterValue,
                     ParameterValueStr = s.ParameterValueStr,
                     UpdatedBy = s.UpdatedBy,
                     TimeUpdated = s.TimeUpdated,
                 })
                    .ToList();
        }

        /// <summary>
        /// 获取指定参数 ID 列表的全局参数表
        /// </summary>
        /// <param name="ids">参数 ID 列表</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByParamID(
            int[] ids,
            long sysLogID)
        {
            return GetByParamID(0, ids, sysLogID);
        }

        /// <summary>
        /// 新增一个社区相关的参数
        /// </summary>
        /// <remarks>本方法只能新增社区标识非0的参数，并且新增参数的标识必须包含在全局参数标识内</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="src">参数 DTO 对象</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Add(
            int communityID,
            IRAPParameterDTO src,
            long sysLogID)
        {
            return Add(
                communityID,
                src.ParameterID,
                src.ParameterValue,
                src.ParameterValueStr,
                sysLogID);
        }

        /// <summary>
        /// 新增一个社区相关的参数
        /// </summary>
        /// <remarks>本方法只能新增社区标识非0的参数，并且新增参数的标识必须包含在全局参数标识内</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="paramID">指定社区新增参数的全局参数标识</param>
        /// <param name="paramValue">参数值（整型）</param>
        /// <param name="paramStrValue">参数值（字符串）</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Add(
            int communityID,
            byte paramID,
            int paramValue,
            string paramStrValue,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            if (communityID == 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = "不能新增社区标识为[0]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            rlt = GetUserInfoWithSysLogID(communityID, sysLogID, out LoginEntity loginInfo);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }

            List<IRAPParameterDTO> lists =
                GetByParamID(communityID, new int[] { paramID }, sysLogID);
            if (lists.Count > 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"社区[{communityID}]中已经存在[ParamID={paramID}]的参数";

                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");

                return rlt;
            }

            lists = GetByParamID(0, new int[] { paramID }, sysLogID);
            if (lists.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"未找到[ParameterID={paramID}]的全局参数";

                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");

                return rlt;
            }

            IRAPParameterEntity entity = new IRAPParameterEntity()
            {
                ParameterID = lists[0].ParameterID,
                ParameterNameID = lists[0].ParameterNameID,
                PartitioningKey = communityID * 10000,
                ParameterValue = paramValue,
                ParameterValueStr = paramStrValue,
                UpdatedBy = loginInfo.UserCode,
                TimeUpdated = DateTime.Now,
            };

            try
            {
                irapParams.Insert(entity);
                irapParams.SaveChanges();
                rlt.ErrCode = 0;
                rlt.ErrText = "参数新增成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"新增参数发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"新增参数发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }

        /// <summary>
        /// 更新参数值
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="paramID">参数标识</param>
        /// <param name="paramValue">参数值（整型）</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Modify(
            int communityID,
            byte paramID,
            int paramValue,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            #region 根据系统登录标识获取登录信息
            LoginEntity loginInfo = null;
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
                rlt.ErrCode = 9999;

                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }
            #endregion

            List<IRAPParameterDTO> iParams =
                GetByParamID(communityID, new int[] { paramID }, sysLogID);
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            try
            {
                irapParams.Update(
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = iParams[0].PartitioningKey,
                        ParameterID = iParams[0].ParameterID,
                        ParameterNameID = iParams[0].ParameterNameID,
                        ParameterValue = paramValue,
                        ParameterValueStr = iParams[0].ParameterValueStr,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    });
                irapParams.SaveChanges();
                rlt.ErrCode = 0;
                rlt.ErrText = "更新参数值（整型）成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"更新参数值发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"更新参数值发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }

        /// <summary>
        /// 更新参数值
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="paramID">参数标识</param>
        /// <param name="paramStrValue">参数值（字符串）</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Modify(
            int communityID,
            byte paramID,
            string paramStrValue,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            rlt =
                GetUserInfoWithSysLogID(
                    communityID,
                    sysLogID,
                    out LoginEntity loginInfo);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }

            List<IRAPParameterDTO> iParams =
                GetByParamID(communityID, new int[] { paramID }, sysLogID);
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            try
            {
                irapParams.Update(
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = iParams[0].PartitioningKey,
                        ParameterID = iParams[0].ParameterID,
                        ParameterNameID = iParams[0].ParameterNameID,
                        ParameterValue = iParams[0].ParameterValue,
                        ParameterValueStr = paramStrValue,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    });
                irapParams.SaveChanges();
                rlt.ErrCode = 0;
                rlt.ErrText = "更新参数值（字符串）成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"更新参数值发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"更新参数值发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }

        /// <summary>
        /// 更新参数值
        /// </summary>
        /// <remarks>本方法用于同时更新参数整型值和字符串值</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="param">参数DTO对象</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Modify(
            int communityID,
            IRAPParameterDTO param,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            rlt =
                GetUserInfoWithSysLogID(
                    communityID,
                    sysLogID,
                    out LoginEntity loginInfo);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }

            List<IRAPParameterDTO> iParams =
                GetByParamID(communityID, new int[] { param.ParameterID }, sysLogID);
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{param.ParameterID}]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            try
            {
                irapParams.Update(
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = param.PartitioningKey,
                        ParameterID = param.ParameterID,
                        ParameterNameID = param.ParameterNameID,
                        ParameterValue = param.ParameterValue,
                        ParameterValueStr = param.ParameterValueStr,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    });
                irapParams.SaveChanges();

                rlt.ErrCode = 0;
                rlt.ErrText = "更新参数成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"更新参数发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"更新参数发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }

        /// <summary>
        /// 删除一个指定的系统参数
        /// </summary>
        /// <remarks>只能删除社区相关的参数，不能删除全局参数</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="param">参数对象</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Delete(
            int communityID,
            IRAPParameterDTO param,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            if (communityID == 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"不能删除社区标识未[0]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
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

            List<IRAPParameterDTO> iParams =
                GetByParamID(communityID, new int[] { param.ParameterID }, sysLogID);
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{param.ParameterID}]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            try
            {
                irapParams.Delete(
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = param.PartitioningKey,
                        ParameterID = param.ParameterID,
                        ParameterNameID = param.ParameterNameID,
                        ParameterValue = param.ParameterValue,
                        ParameterValueStr = param.ParameterValueStr,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    },
                    false);
                irapParams.SaveChanges();
                rlt.ErrCode = 0;
                rlt.ErrText = "删除参数成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"删除参数发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"删除参数发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }

        /// <summary>
        /// 删除一个指定的系统参数
        /// </summary>
        /// <remarks>只能删除社区相关的参数，不能删除全局参数</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="paramID">参数标识</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Delete(
            int communityID,
            byte paramID,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

            if (communityID == 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"不能删除社区标识未[0]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
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

            List<IRAPParameterDTO> iParams =
                GetByParamID(communityID, new int[] { paramID }, sysLogID);
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
                return rlt;
            }

            try
            {
                irapParams.Delete(
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = iParams[0].PartitioningKey,
                        ParameterID = iParams[0].ParameterID,
                        ParameterNameID = iParams[0].ParameterNameID,
                        ParameterValue = iParams[0].ParameterValue,
                        ParameterValueStr = iParams[0].ParameterValueStr,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    },
                    false);
                irapParams.SaveChanges();
                rlt.ErrCode = 0;
                rlt.ErrText = "删除参数成功";
            }
            catch (Exception error)
            {
                rlt.ErrCode = 9999;
                if (error.InnerException.InnerException != null)
                {
                    rlt.ErrText =
                        $"删除参数发生异常：" +
                        $"{error.InnerException.InnerException.Message}";
                }
                else
                {
                    rlt.ErrText = $"删除参数发生异常：{error.Message}";
                }
                Log.Instance.WriteMsg<IRAPParameterSet>(
                    LogType.ERROR,
                    $"[({rlt.ErrCode}){rlt.ErrText}]");
            }

            return rlt;
        }
    }
}
