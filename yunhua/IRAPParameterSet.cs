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
        /// 申请一个参数标识号
        /// </summary>
        /// <param name="paramID">输出参数，申请到的参数标识号</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        private IRAPError RequestParameterID(out int paramID)
        {
            #region 获取 ParameterID
            string sequenceName = "NextParameterID";
            SequenceValueDTO rtnSequence =
                IRAPSequence.GetSequence(sequenceName, 1);
            if (rtnSequence.ErrCode != 0)
            {
                string msg = rtnSequence.ErrText;
                Log.InstanceID.WriteMsg<IRAPParameterSet>(Enums.LogType.ERROR, msg);

                paramID = 0;
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = rtnSequence.ErrText,
                    };
            }
            else
            {
                paramID = (int)rtnSequence.SequenceValue;
                Log.InstanceID.WriteMsg<IRAPParameterSet>(
                    Enums.LogType.DEBUG,
                    $"获得ParameterID={paramID}");
                return
                    new IRAPError()
                    {
                        ErrCode = 0,
                        ErrText = "申请参数标识成功",
                    };
            }
            #endregion
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
            }
            #endregion

            return rlt;
        }

        /// <summary>
        /// 获取 IRAP 全局参数表
        /// </summary>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetGlobal()
        {
            return GetByCommunityID(0);
        }

        /// <summary>
        /// 获取社区相关的参数表
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByCommunityID(int communityID)
        {
            IQueryable<IRAPParameterEntity> dtParams = irapParams.Table;
            IQueryable<SysNameSpaceEntity> dtNames = names.Table;

            return
                (from s in
                    dtParams
                        .Where(p => p.PartitioningKey == communityID * 10000)
                 join l in
                     dtNames
                         .Where(
                             t => t.LanguageID == 30 &&
                                (t.PartitioningKey == communityID * 10000 ||
                                 t.PartitioningKey == 0))
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
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByParamID(
            int communityID,
            int[] ids)
        {
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
                            t => t.LanguageID == 30 &&
                                (t.PartitioningKey == communityID * 10000 ||
                                 t.PartitioningKey == 0))
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
        /// <returns>系统参数实体对象集合</returns>
        public List<IRAPParameterDTO> GetByParamID(
            int[] ids)
        {
            return GetByParamID(0, ids);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <remarks>本方法需要在序列服务器中配置NextParameterID序列</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="src">参数 DTO 对象</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="paramID">输出参数，新增参数的参数标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Add(
            int communityID,
            IRAPParameterDTO src,
            long sysLogID,
            out int paramID)
        {
            return Add(
                communityID,
                src.ParameterName,
                src.ParameterValue,
                src.ParameterValueStr,
                sysLogID,
                out paramID);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <remarks>本方法需要在序列服务器中配置NextParameterID序列</remarks>
        /// <param name="communityID">社区标识</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值（整型）</param>
        /// <param name="paramStrValue">参数值（字符串）</param>
        /// <param name="sysLogID">系统登录标识</param>
        /// <param name="paramID">输出参数，新增参数的参数标识</param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Add(
            int communityID,
            string paramName,
            int paramValue,
            string paramStrValue,
            long sysLogID,
            out int paramID)
        {
            IRAPError rlt = new IRAPError();
            paramID = 0;

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

                return rlt;
            }

            IIRAPNamespaceSet namespaceSet =
                IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            rlt =
                namespaceSet.Add(
                    communityID,
                    paramName,
                    loginInfo.LanguageID,
                    out int nameID);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }

            rlt = RequestParameterID(out paramID);
            if (rlt.ErrCode != 0)
            {
                return rlt;
            }

            IRAPParameterEntity entity = new IRAPParameterEntity()
            {
                ParameterID = (byte)paramID,
                ParameterNameID = nameID,
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

                return rlt;
            }
            #endregion

            List<IRAPParameterDTO> iParams = GetByParamID(communityID, new int[] { paramID });
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
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

            List<IRAPParameterDTO> iParams = GetByParamID(communityID, new int[] { paramID });
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText = $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
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
            }

            return rlt;
        }

        /// <summary>
        /// 修改参数名称
        /// </summary>
        /// <remarks>本方法主要用于更新参数名称，也可以在此同时更新参数值</remarks>
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
                GetByParamID(communityID, new int[] { param.ParameterID });
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{param.ParameterID}]的参数";
                return rlt;
            }

            IIRAPNamespaceSet namespaceSet =
                IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            if (param.ParameterNameID == 0)
            {
                rlt =
                    namespaceSet.Add(
                        communityID,
                        param.ParameterName,
                        loginInfo.LanguageID,
                        out int nameID);
                if (rlt.ErrCode != 0)
                {
                    return rlt;
                }
                else
                {
                    param.ParameterNameID = nameID;
                }
            }

            try
            {
                IRAPParameterEntity entity =
                    new IRAPParameterEntity()
                    {
                        PartitioningKey = param.PartitioningKey,
                        ParameterID = param.ParameterID,
                        ParameterNameID = param.ParameterNameID,
                        ParameterValue = param.ParameterValue,
                        ParameterValueStr = param.ParameterValueStr,
                        UpdatedBy = loginInfo.UserCode,
                        TimeUpdated = DateTime.Now,
                    };
                db.Entry(entity).State = EntityState.Modified;
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
            }

            return rlt;
        }

        /// <summary>
        /// 删除一个指定的系统参数
        /// </summary>
        /// <param name="communityID"></param>
        /// <param name="param"></param>
        /// <param name="sysLogID"></param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Delete(
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
                GetByParamID(communityID, new int[] { param.ParameterID });
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{param.ParameterID}]的参数";
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
                    });
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
            }

            return rlt;
        }

        /// <summary>
        /// 删除一个指定的系统参数
        /// </summary>
        /// <param name="communityID"></param>
        /// <param name="paramID"></param>
        /// <param name="sysLogID"></param>
        /// <returns>IRAP系统通用错误对象，如果其中的ErrCode：0-执行成功；非0执行失败</returns>
        public IRAPError Delete(
            int communityID,
            byte paramID,
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
                GetByParamID(communityID, new int[] { paramID });
            if (iParams.Count <= 0)
            {
                rlt.ErrCode = 9999;
                rlt.ErrText =
                    $"[{communityID}]社区中未找到ParameterID=[{paramID}]的参数";
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
                    });
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
            }

            return rlt;
        }

        // ToDo: 今天就写到这里，明天继续 20190703
    }
}
