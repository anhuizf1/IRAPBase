using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using System.Security.Cryptography;
using IRAPBase.DTO;
using System.Text.RegularExpressions;
using IRAPCommon;
using System.Configuration;
using System.Data;
//using System.Text;

namespace IRAPBase
{
    /// <summary>
    /// 对单用户操作的类
    /// </summary>
    public class IRAPUser
    {
        private string _userCode = string.Empty;
        private long _communityID = 0;
        private UnitOfWork _unitOfWork;
        private IRAPUserEntity user = null;
        private Repository<IRAPUserEntity> _users;
        /// <summary>
        /// 分区键值CommunityID*10000
        /// </summary>
        public long PK { get { return _communityID * 10000L; } }

        /// <summary>
        /// 此用户的实体对象
        /// </summary>
        public IRAPUserEntity User { get { return user; } }

        //此用户的用户集合
        public Repository<IRAPUserEntity> Repository { get { return _users; } }

        #region 构造函数
        /*
        public IRAPUser()
        {

            _unitOfWork = new UnitOfWork(new IRAPSqlDBContext("IRAPContext"));
            _users = _unitOfWork.Repository<IRAPUserEntity>();
        }*/
        /// <summary>
        /// 用户类的构造函数
        /// </summary>
        /// <param name="userCode">用户代码</param>
        /// <param name="communityID">社区标识</param>
        public IRAPUser(string userCode, long communityID)
        {
            this._userCode = userCode;
            this._communityID = communityID;

            _unitOfWork = new UnitOfWork(new IRAPSqlDBContext("IRAPContext"));
            _users = _unitOfWork.Repository<IRAPUserEntity>();
            IRAPUserEntity e = _users.Entities.FirstOrDefault(r => r.UserCode == _userCode && r.PartitioningKey == PK);
            if (e != null)
            {
                user = e;
            }
        }

        #endregion

        /// <summary>
        /// 修改用户的实体User调用此方法保存生效
        /// </summary>
        /// <returns></returns>
        public IRAPError Modify()
        {

            if (user == null)
            {
                return new IRAPError(22, $"用户名{_userCode}无效！");
            }
            _users.Update(user);
            int resInt = _users.SaveChanges();
            if (resInt > 0)
            {
                return new IRAPError(0, "修改成功！");
            }
            else
            {
                return new IRAPError(22, "修改失败，属性未发生变化无需修改！");
            }
        }

        /// <summary>
        /// 重置密码为123456
        /// </summary>
        /// <param name="pwd">重置的密码</param>
        /// <returns></returns>
        public IRAPError ResetPWD(string pwd)
        {
            user.EncryptedPWD = GetDBBinaryPassword(pwd);
             
            int resInt = _users.SaveChanges();
            if (resInt > 0)
            {
                return new IRAPError(0, "重置密码成功！");
            }
            else
            {
                return new IRAPError(22, "重置密码失败，属性未发生变化无需修改！");
            }
        }
        /// <summary>
        /// 删除此用户
        /// </summary>
        /// <returns></returns>
        public IRAPError Delete()
        {
            if (user == null)
            {
                return new IRAPError(221, "用户不存在！");
            }
            _users.Delete(user);
            int resInt = _users.SaveChanges();
            if (resInt > 0)
            {
                return new IRAPError(0, "删除成功！");
            }
            else
            {
                return new IRAPError(22, "删除失败！");
            }
        }

        #region //获取机构清单

        /// <summary>
        /// 获取此用户所属机构清单
        /// </summary>
        /// <returns></returns>
        public BackLeafSetDTO GetAgencyList()
        {
            BackLeafSetDTO backRes = new BackLeafSetDTO();
            if (user == null)
            {
                backRes.ErrCode = 11;
                backRes.ErrText = "找不到该用户：" + _userCode;

                return backRes;
            }

            string[] agencyArray = user.AgencyNodeList.Split(',');
            List<int> agencyArrayInt = new List<int>();
            foreach (string item in agencyArray)
            {
                agencyArrayInt.Add(-int.Parse(item));
            }
            Repository<ETreeSysLeaf> agecies = _unitOfWork.Repository<ETreeSysLeaf>();

            var agencyList = agecies.Table.Where(u => agencyArrayInt.Contains(u.LeafID) && u.PartitioningKey == PK + 1 && u.TreeID == 1);
            backRes.Rows = new List<LeafDTO>();
            foreach (var row in agencyList)
            {
                LeafDTO d = new LeafDTO
                {
                    Leaf = row.LeafID,
                    Code = row.Code,
                    Name = row.NodeName
                };
                backRes.Rows.Add(d);
            }
            backRes.ErrCode = 0;
            backRes.ErrText = "信息获取成功！";
            return backRes;
        }
        #endregion
        #region //获取角色清单

        /// <summary>
        /// 获取此用户所属角色清单
        /// </summary>
        /// <returns></returns>
        public BackLeafSetDTO GetRoleList()
        {
            BackLeafSetDTO backRes = new BackLeafSetDTO();
            if (user == null)
            {
                backRes.ErrCode = 11;
                backRes.ErrText = "找不到该用户：" + _userCode;

                return backRes;
            }

            string[] roleArray = user.RoleNodeList.Split(',');
            List<int> roleArrayInt = new List<int>();
            foreach (string item in roleArray)
            {
                roleArrayInt.Add(-int.Parse(item));
            }
            Repository<ETreeSysLeaf> agecies = _unitOfWork.Repository<ETreeSysLeaf>();

            var agencyList = agecies.Table.Where(u => roleArrayInt.Contains(u.LeafID) && u.TreeID == 2);
            backRes.Rows = new List<LeafDTO>();
            foreach (var row in agencyList)
            {
                LeafDTO d = new LeafDTO
                {
                    Leaf = row.LeafID,
                    Code = row.Code,
                    Name = row.NodeName
                };
                backRes.Rows.Add(d);
            }
            backRes.ErrCode = 0;
            backRes.ErrText = "信息获取成功！";
            return backRes;
        }

        #endregion

        #region //用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="clientID">渠道标识</param>
        /// <param name="plainPWD">密码（明文）</param>
        /// <param name="veriCode">验证码</param>
        /// <param name="stationID">站点标识，如果是BS系统调用传CommunityID</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="agencyLeaf">机构标识</param>
        /// <param name="roleLeaf">角色标识</param>
        /// <returns>返回信息DTO类</returns>
        public BackLoginInfo Login(string clientID, string plainPWD, string veriCode, string stationID, string ipAddress, int agencyLeaf, int roleLeaf)
        {
            BackLoginInfo backRes = new BackLoginInfo
            {
                UserName = user.UserName,
                LanguageID = user.LanguageID,
                NickName = user.UserEnglishName,
                MPhoneNo = user.MPhoneNo,
                OPhoneNo = user.OPhoneNo,
                HPhoneNo = user.HPhoneNo
            };
            try
            {
                //验证密码
                IRAPError error = VerifyPWD(plainPWD);
                if (error.ErrCode != 0)
                {
                    backRes.ErrCode = error.ErrCode;
                    backRes.ErrText = error.ErrText;
                    return backRes;
                }
                //判断信息站点是否注册
                if (!Regex.IsMatch(stationID, @"^\d+$"))
                {
                    IRAPStation station = new IRAPStation();
                    StationEntity r = station.GetStation(stationID);
                    if (r == null)
                    {
                        backRes.ErrCode = 9999;
                        backRes.ErrText = $"站点{stationID}不存在！";
                        return backRes;
                    }
                    backRes.HostName = r.HostName;
                }
                //短信验证码是否有效
                //申请登录标识

                long sysLogID = IRAPSequence.GetSysLogID();
                //登录
                Repository<LoginEntity> loginRep = _unitOfWork.Repository<LoginEntity>();
                LoginEntity loginEntity = new LoginEntity();
                loginEntity.PartitioningKey = PK;
                loginEntity.ClientID = clientID;
                loginEntity.SysLogID = (int)sysLogID;
                loginEntity.UserCode = _userCode;
                loginEntity.AgencyLeaf = agencyLeaf;
                loginEntity.RoleLeaf = roleLeaf;
                loginEntity.Access_Token = Guid.NewGuid().ToString();
                loginEntity.StationID = stationID;
                loginEntity.MPhoneNo = user.MPhoneNo;
                loginEntity.Status = 1;
                loginEntity.LanguageID = user.LanguageID;
                loginEntity.IPAddress = ipAddress;

                loginEntity.LoginMode = 1;
                loginRep.Insert(loginEntity);
                _unitOfWork.Commit();

                backRes.SysLogID = sysLogID;
                backRes.access_token = loginEntity.Access_Token;
                backRes.AgencyName = _unitOfWork.Repository<ETreeSysLeaf>().Table.FirstOrDefault(r => r.LeafID == agencyLeaf).NodeName;
                backRes.ErrCode = 0;
                backRes.ErrText = $"登录成功：{sysLogID}";


                return backRes;
            }
            catch (Exception err)
            {
                backRes.ErrCode = 9999;
                backRes.ErrText = $"登录遇到异常:{err.Message}";

                return backRes;
            }
        }
        #endregion
        #region  
        /// <summary>
        /// 验证密码类
        /// </summary>
        /// <param name="pwd">密码（明文）</param>
        /// <returns>验证结果</returns>
        public IRAPError VerifyPWD(string pwd)
        {
            IRAPError error = new IRAPError();
            if (Encoding.UTF8.GetString(user.EncryptedPWD) == IRAPCommon.IRAPMD5.MD5(pwd))
            {
                error.ErrCode = 0;
                error.ErrText = "密码正确！";
            }
            else
            {
                error.ErrCode = 22;
                error.ErrText = "密码错误！";
            }
            return error;
        }

        #endregion


        #region  
        /// <summary>
        /// 密码md5算法(用户密码加密默认此方法)
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static byte[] GetBinaryPassword(string passWord)
        {
            string backRes = IRAPCommon.IRAPMD5.MD5(passWord);
            return Encoding.UTF8.GetBytes(backRes);
            //sfn_GetBinaryOfUserPassword
        }

        /// <summary>
        /// DES加密算法
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static byte[] GetBinaryPasswordDES(string passWord)
        {

            string backRes = IRAPCommon.IRAPDES.EncryptDES(passWord, "chinairapchinairap");

            return Encoding.UTF8.GetBytes(backRes);
            //sfn_GetBinaryOfUserPassword
        }
        /// <summary>
        /// AES算法
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static byte[] GetBinaryPasswordAES(string passWord)
        {
            string backRes = IRAPCommon.IRAPAES.Encrypt(passWord, "chinairap", "chinairap");
            return Encoding.UTF8.GetBytes(backRes);
            //sfn_GetBinaryOfUserPassword
        }

        /// <summary>
        /// SQLServer专用方法返回密码
        /// </summary>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public static byte[] GetDBBinaryPassword(string passWord)
        {
            var DB = DBContextFactory.Instance.CreateContext("IRAPContext");
            var pwdlist = DB.DataBase.SqlQuery<byte[]>("select IRAP.dbo.sfn_GetBinaryOfUserPassword(@PlainPWD) as PWD",
                 new System.Data.SqlClient.SqlParameter("@PlainPWD", passWord));
           var ojb= pwdlist.FirstOrDefault();
            if (ojb!=null)
            {
                return ojb;
            }
            else
            {
                return new byte[10];
            }

            //sfn_GetBinaryOfUserPassword
        }
        #endregion



    }
}
