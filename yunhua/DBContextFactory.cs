using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace IRAPBase
{
    /// <summary>
    /// 创建数据库连接上下文的工厂类, 这是一个单例类
    /// </summary>
    public class DBContextFactory
    {
        #region 单例类
        private static DBContextFactory _instance = null;

        public static DBContextFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBContextFactory();
                }
                return _instance;
            }
        }
        #endregion

        // 数据库类型，默认: 1-SQLServer
        private string dbType = "1";

        private DBContextFactory()
        {
            if (ConfigurationManager.AppSettings["DBType"] != null)
            {
                dbType = ConfigurationManager.AppSettings["DBType"];
            }
        }

        /// <summary>
        /// 创建数据库连接上下文
        /// </summary>
        /// <param name="contextName">
        /// 上下文名称，数据库类型是 SQLServer 时，必须有
        /// </param>
        /// <returns></returns>
        public IDbContext CreateContext(string contextName = "")
        {
            switch (dbType)
            {
                case "1":
                    if (string.IsNullOrEmpty(contextName))
                    {
                        throw new Exception("数据库连接上下文名称不能空白");
                    }

                    try
                    {
                        return new IRAPSqlDBContext(contextName);
                    }
                    catch (Exception error)
                    {
                        throw new Exception(
                            $"创建数据库连接上下文失败：{error.Message}");
                    }
                case "2":
                    throw new Exception("目前暂时不支持 Oracle 数据库连接");
                case "3":
                    try
                    {
                        return new IRAPMyDBContext();
                    }
                    catch (Exception error)
                    {
                        throw new Exception(
                            $"创建数据库连接上下文失败：{error.Message}");
                    }
                default:
                    throw new Exception($"未知的数据库类型: [{dbType}]");
            }
        }
    }
}
