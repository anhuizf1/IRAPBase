using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{

    /// <summary>
    /// 对用户的批量操作
    /// </summary>
    public class IRAPUserSet
    {
        private long _communityID = 0;
        private Repository<IRAPUserEntity> _users;

        private long PK { get { return _communityID * 10000L; } }

        /// <summary>
        /// 用户集合
        /// </summary>
        public IQueryable<IRAPUserEntity> UserSet { get { return _users.Table.Where(c=>c.PartitioningKey==PK); } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityID">社区标识</param>
        public IRAPUserSet(int communityID)
        {
            _communityID = communityID;
            _users = new Repository<IRAPUserEntity>("IRAPContext");
        }

        #region //新增用户
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="userCode">用户代码</param>
        /// <param name="userName">用户名称</param>
        /// <param name="passwd">密码（明文）</param>
        /// <param name="agencyNodeList">机构清单用逗号隔开，例如：-1,-2,-3</param>
        /// <param name="roleNodeList">角色清单用逗号隔开，例如：-4,-5,-6</param>
        /// <returns>通用错误</returns>
        public IRAPError AddUser(string userCode, string userName, string passwd, string agencyNodeList, string roleNodeList)
        {
            IRAPError error = new IRAPError();
            IRAPUserEntity e = new IRAPUserEntity();
            try
            {
                e.PartitioningKey = PK;
                e.UserCode = userCode;
                e.UserName = userName;
                e.EncryptedPWD = IRAPUser.GetBinaryPassword(passwd);
                e.AgencyNodeList = agencyNodeList;
                e.RoleNodeList = roleNodeList;
                e.RegistedTime = DateTime.Now;
                e.ModifiedTime = DateTime.Now;
                _users.Insert(e);
                _users.SaveChanges();
                error.ErrCode = 0;
                error.ErrText = "增加用户成功！";
                return error;
            }
            catch (Exception err)
            {
                if (err.InnerException.InnerException != null)
                {
                    error.ErrText = "增加用户发生异常：" + err.InnerException.InnerException.Message;
                }
                else
                {
                    error.ErrText = "增加用户发生异常：" + err.Message;
                }
                error.ErrCode = 9999;

                return error;
            }
        }

        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public IRAPError AddUser(IRAPUserEntity e)
        {
            IRAPError error = new IRAPError();
            try
            {
                e.RegistedTime = DateTime.Now;
                e.ModifiedTime = DateTime.Now;

                _users.Insert(e);
                _users.SaveChanges();
                error.ErrCode = 0;
                error.ErrText = "增加用户成功！";
                return error;
            }
            catch (Exception err)
            {
                if (err.InnerException.InnerException != null)
                {
                    error.ErrText = "增加用户发生异常：" + err.InnerException.InnerException.Message;
                }
                else
                {
                    error.ErrText = "增加用户发生异常：" + err.Message;
                }
                error.ErrCode = 9999;

                return error;
            }
        }

        #endregion

        #region  
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="e">用户实体，此实体必须是从资料库中查出来的实体</param>
        /// <returns>通用错误</returns>
        public IRAPError ModifyUser(IRAPUserEntity e)
        {
            IRAPError error = new IRAPError();
            _users.Update(e);
            _users.SaveChanges();
            error.ErrCode = 0;
            error.ErrText = "修改成功！";
            return error;
        }

        #endregion

        #region 
        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="userCode">用户代码</param>
        /// <returns></returns>
        public IRAPError DeleteUser( string userCode)
        {
            IRAPError error = new IRAPError();
            IRAPUserEntity e = _users.Table.FirstOrDefault(r => r.PartitioningKey == PK && r.UserCode == userCode);
            if (e == null)
            {
                error.ErrCode = 9999;
                error.ErrText = "用户代码不存在！" + userCode;
                return error;
            }
            _users.Delete(e, false);
            if (_users.SaveChanges() > 0)
            {
                error.ErrCode = 0;
                error.ErrText = "删除成功！";
            }
            else
            {
                error.ErrCode = 91;
                error.ErrText = "删除失败！";
            }
            return error;
        }

        #endregion

        #region 批量业务接口(待修改,可以通过属性UserRepo扩展修改)
        /// <summary>
        /// 批量插入用户
        /// </summary>
        /// <param name="userList">用户清单</param>
        /// <returns>IRAP通用错误</returns>
        public IRAPError BatchImportUser(List<IRAPUserEntity> userList)
        {
            IRAPError error = new IRAPError(1, "初始化错误。");
            try
            {
                foreach (IRAPUserEntity r in userList)
                {
                    _users.Insert(r);
                }
                if (_users.SaveChanges() > 0)
                {
                    error.ErrCode = 0;
                    error.ErrText = "导入成功！";
                }
                else
                {
                    error.ErrCode = 113;
                    error.ErrText = "导入记录为0，请检查集合中是否有记录！";
                }
                return error;
            }
            catch (Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"批量导入用户错误：{err.Message}";
                return error;
            }

        }
        #endregion 

        /// <summary>
        /// 根据用户代码查询用户实体对象
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="userCode">用户代码</param>
        /// <returns></returns>
        public static IRAPUserEntity GetUserEntityByCode(int communityID, string userCode)
        {
            IDbContext db = DBContextFactory.Instance.CreateContext("IRAPContext");
            return db.Set<IRAPUserEntity>().FirstOrDefault(c => c.PartitioningKey == communityID * 10000L && c.UserCode == userCode);
        }

        /// <summary>
        /// 根据部门查找用户清单
        /// </summary>
        /// <param name="agencyLeaf"></param>
        /// <returns></returns>
        public List<IRAPUserEntity>  GetUserListByAgency(int agencyLeaf)
        {
            string keyName = "-" + agencyLeaf + "";
            return _users.Table.Where(  c => c.PartitioningKey==PK&& c.AgencyNodeList.Contains(keyName)).ToList();
        }
    }
}
