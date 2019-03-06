using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IRAPBase.Entities;
using IRAPBase.DTO;

namespace IRAPBase
{
    public class IRAPParameterSet
    {
        private Repository<IRAPParameterEntity> irapParams = null;
        private Repository<SysNameSpaceEntity> names = null;
        private IDbContext db = null;

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
        /// 获取 IRAP 全局参数表
        /// </summary>
        /// <returns></returns>
        public List<IRAPParameterDTO> Global()
        {
            return ByCommunityID(0);
        }

        /// <summary>
        /// 获取社区相关的参数表
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <returns></returns>
        public List<IRAPParameterDTO> ByCommunityID(int communityID)
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
                             t => t.PartitioningKey == communityID * 10000 &&
                             t.LanguageID == 30)
                     on s.ParameterID equals l.NameID
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
        /// <returns></returns>
        public List<IRAPParameterDTO> ByParamID(
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
                            t => t.PartitioningKey == communityID * 10000 &&
                            t.LanguageID == 30)
                    on s.ParameterID equals l.NameID
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
        /// <returns></returns>
        public List<IRAPParameterDTO> ByParamID(
            int[] ids)
        {
            return ByParamID(0, ids);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="src">参数 DTO 对象</param>
        /// <param name="sysLogID">用户</param>
        /// <returns></returns>
        public IRAPError Add(
            int communityID,
            IRAPParameterDTO src,
            long sysLogID)
        {
            IRAPError rlt = new IRAPError();

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

            int nameID =
                IRAPNamespaceSet.Instance.Add(
                    communityID,
                    src.ParameterName);

            IRAPParameterEntity entity = new IRAPParameterEntity()
            {
                ParameterID = src.ParameterID,
                ParameterNameID = nameID,
                PartitioningKey = communityID * 10000,
                ParameterValue = src.ParameterValue,
                ParameterValueStr = src.ParameterValueStr,
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
    }
}
