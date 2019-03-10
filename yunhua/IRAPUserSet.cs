using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    //对用户的批量操作
    public class IRAPUserSet
    {
        private long _communityID = 0;
        private Repository<IRAPUserEntity> _users;

        public long PK { get { return _communityID * 10000L; } }

        public Repository<IRAPUserEntity> UserRep { get { return _users; } }

        public IRAPUserSet()
        {
            _users = new Repository<IRAPUserEntity>("IRAPContext");
        }

        #region //新增用户
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

        #region //修改用户

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

        #region //删除指定用户
        public IRAPError DeleteUser(long pk, string userCode)
        {
            IRAPError error = new IRAPError();
            IRAPUserEntity e = _users.Table.FirstOrDefault(r => r.PartitioningKey == pk && r.UserCode == userCode);
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
    }
}
