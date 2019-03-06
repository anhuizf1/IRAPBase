using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IRAPBase.Entities;
using IRAPBase.Enums;
using IRAPBase.DTO;

namespace IRAPBase
{
    public interface IIRAPNamespaceSet
    {
        /// <summary>
        /// 查找指定名称的名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文)</param>
        /// <returns>名称标识</returns>
        int GetNameID(
            string nameDescription,
            int languageID = 30);

        /// <summary>
        /// 新增一个名称，并返回名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文) </param>
        /// <returns>名称标识</returns>
        int Add(
            string nameDescription,
            int languageID = 30);

        /// <summary>
        /// 修改名称
        /// </summary>
        /// <param name="src">名称对象</param>
        /// <returns></returns>
        IRAPError Modify(NameSpaceEntity src);

        /// <summary>
        /// 删除指定的名称
        /// </summary>
        /// <param name="nameID">名称标识</param>
        /// <param name="languageID">语言标识</param>
        /// <returns></returns>
        IRAPError Delete(int nameID, byte languageID = 30);

        /// <summary>
        /// 删除指定的名称
        /// </summary>
        /// <param name="src">名称实体对象</param>
        /// <returns></returns>
        IRAPError Delete(NameSpaceEntity src);

        /// <summary>
        /// 获取指定名称标识和语言标识的名称实体对象
        /// </summary>
        /// <param name="nameID">名称标识</param>
        /// <param name="languageID">语言标识</param>
        /// <returns></returns>
        NameSpaceEntity Get(int nameID, byte languageID);

        /// <summary>
        /// 获取指定语言标识的名称实体对象列表
        /// </summary>
        /// <param name="languageID">语言标识</param>
        /// <returns></returns>
        List<NameSpaceEntity> Get(byte languageID);
    }

    public class IRAPNamespaceSetFactory
    {
        public static IIRAPNamespaceSet CreatInstance(
            NamespaceType namespaceType)
        {
            switch (namespaceType)
            {
                case NamespaceType.Sys:
                    return new IRAPSysNamespaceSet();
                case NamespaceType.Biz:
                    return new IRAPBizNamespaceSet();
                default:
                    return null;
            }
        }
    }

    internal class IRAPSysNamespaceSet : IIRAPNamespaceSet
    {
        public IRAPSysNamespaceSet() { }

        /// <summary>
        /// 生成 SysNameSpaceEntity 的资源库 
        /// </summary>
        /// <returns></returns>
        private Repository<SysNameSpaceEntity> GetRepository()
        {
            try
            {
                IDbContext db =
                    DBContextFactory
                        .Instance
                        .CreateContext("IRAPContext");
                return new Repository<SysNameSpaceEntity>(db);
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        /// <summary>
        /// 查找指定名称的名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文)</param>
        /// <returns>名称标识</returns>
        public int GetNameID(
            string nameDescription,
            int languageID = 30)
        {
            Repository<SysNameSpaceEntity> names = null;

            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                Console.WriteLine(
                    $"获取资源库的时候发生错误: {error.Message}");
                return 0;
            }

            NameSpaceEntity entity =
                names
                    .Table
                    .Where(
                        p => p.LanguageID == languageID &&
                        p.NameDescription == nameDescription)
                    .FirstOrDefault();
            if (entity == null)
            {
                return 0;
            }
            else
            {
                return entity.NameID;
            }
        }

        /// <summary>
        /// 新增一个名称，并返回名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文) </param>
        /// <returns>名称标识</returns>
        public int Add(
            string nameDescription,
            int languageID = 30)
        {
            int nameID =
                GetNameID(
                    nameDescription,
                    languageID);
            if (nameID != 0)
            {
                return nameID;
            }

            return 0;
        }

        public IRAPError Modify(NameSpaceEntity src)
        {
            Repository<SysNameSpaceEntity> names = null;
            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }

            if (src is SysNameSpaceEntity)
            {
                try
                {
                    var entity =
                        names.Entities.Find(
                            src.PartitioningKey,
                            src.LanguageID,
                            src.NameID);
                    if (entity == null)
                    {
                        return
                            new IRAPError()
                            {
                                ErrCode = 9999,
                                ErrText =
                                    $"未找到 PartitioningKey={src.PartitioningKey}|" +
                                    $"LanguagID={src.LanguageID}|" +
                                    $"NameID={src.NameID}的记录",
                            };
                    }

                    names.Update(src as SysNameSpaceEntity);
                    names.SaveChanges();

                    return
                        new IRAPError()
                        {
                            ErrCode = 0,
                            ErrText = "更新成功",
                        };
                }
                catch (Exception error)
                {
                    string msg = $"更新内容时发生错误: {error.Message}";

                    Console.WriteLine(msg);
                    return
                        new IRAPError()
                        {
                            ErrCode = 9999,
                            ErrText = msg,
                        };
                }
            }
            else
            {
                string msg = $"传入的参数不是 SysNameSpaceEntity 对象";

                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }
        }

        public IRAPError Delete(int nameID, byte languageID = 30)
        {
            return
                Delete(
                    new SysNameSpaceEntity()
                    {
                        NameID = nameID,
                        LanguageID = languageID,
                    });
        }

        public IRAPError Delete(NameSpaceEntity src)
        {
            Repository<SysNameSpaceEntity> names = null;
            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }

            if (src is SysNameSpaceEntity)
            {
                try
                {
                    var entity =
                        names.Entities.Find(
                            src.PartitioningKey, 
                            src.LanguageID, 
                            src.NameID);
                    if (entity == null)
                    {
                        return
                            new IRAPError()
                            {
                                ErrCode = 9999,
                                ErrText = 
                                    $"未找到 PartitioningKey={src.PartitioningKey}|" +
                                    $"LanguagID={src.LanguageID}|" +
                                    $"NameID={src.NameID}的记录",
                            };
                    }

                    names.Delete(entity, false);
                    names.SaveChanges();

                    return
                        new IRAPError()
                        {
                            ErrCode = 0,
                            ErrText = "删除成功",
                        };
                }
                catch (Exception error)
                {
                    string msg = $"删除名称时发生错误: {error.Message}";

                    Console.WriteLine(msg);
                    return
                        new IRAPError()
                        {
                            ErrCode = 9999,
                            ErrText = msg,
                        };
                }
            }
            else
            {
                string msg = $"传入的参数不是 SysNameSpaceEntity 对象";

                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }
        }

        public NameSpaceEntity Get(int nameID, byte languageID = 30)
        {
            IQueryable<NameSpaceEntity> queryable = null;
            try
            {
                Repository<SysNameSpaceEntity> names = GetRepository();
                queryable = names.Table;
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return null;
            }

            var rlt =
                    queryable
                        .Where(
                            p => p.PartitioningKey == 0 &&
                                p.NameID == nameID &&
                                p.LanguageID == languageID).FirstOrDefault();
            return rlt;
        }

        public List<NameSpaceEntity> Get(byte languageID = 30)
        {
            IQueryable<NameSpaceEntity> queryable = null;
            try
            {
                Repository<SysNameSpaceEntity> names = GetRepository();
                queryable = names.Table;
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return null;
            }

            var rlt =
                    queryable
                        .Where(
                            p => p.PartitioningKey == 0 &&
                                p.LanguageID == languageID)
                        .OrderBy(p => p.NameID)
                        .ToList();
            return rlt;
        }
    }

    internal class IRAPBizNamespaceSet : IIRAPNamespaceSet
    {
        public IRAPBizNamespaceSet() { }

        /// <summary>
        /// 生成 BizNameSpaceEntity 的资源库 
        /// </summary>
        /// <returns></returns>
        private Repository<BizNameSpaceEntity> GetRepository()
        {
            try
            {
                IDbContext db =
                    DBContextFactory
                        .Instance
                        .CreateContext("IRAPMDMContext");
                return new Repository<BizNameSpaceEntity>(db);
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        /// <summary>
        /// 查找指定名称的名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文)</param>
        /// <returns>名称标识</returns>
        public int GetNameID(
            string nameDescription,
            int languageID = 30)
        {
            Repository<BizNameSpaceEntity> names = null;

            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                Console.WriteLine(
                    $"获取资源库的时候发生错误: {error.Message}");
                return 0;
            }

            NameSpaceEntity entity =
                names
                    .Table
                    .Where(
                        p => p.LanguageID == languageID &&
                        p.NameDescription == nameDescription)
                    .FirstOrDefault();
            if (entity == null)
            {
                return 0;
            }
            else
            {
                return entity.NameID;
            }
        }

        /// <summary>
        /// 新增一个名称，并返回名称标识
        /// </summary>
        /// <param name="nameDescription">名称</param>
        /// <param name="languageID">语言标识 (默认: 30 简体中文) </param>
        /// <returns>名称标识</returns>
        public int Add(
            string nameDescription,
            int languageID = 30)
        {
            int nameID =
                GetNameID(
                    nameDescription,
                    languageID);
            if (nameID != 0)
            {
                return nameID;
            }

            return 0;
        }

        public IRAPError Modify(NameSpaceEntity src)
        {
            Repository<BizNameSpaceEntity> names = null;
            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }

            if (src is BizNameSpaceEntity)
            {
                try
                {
                    var entity =
                       names.Entities.Find(
                           src.PartitioningKey,
                           src.LanguageID,
                           src.NameID);
                    if (entity == null)
                    {
                        return
                            new IRAPError()
                            {
                                ErrCode = 9999,
                                ErrText =
                                    $"未找到 PartitioningKey={src.PartitioningKey}|" +
                                    $"LanguagID={src.LanguageID}|" +
                                    $"NameID={src.NameID}的记录",
                            };
                    }

                    names.Update(src as BizNameSpaceEntity);
                    names.SaveChanges();

                    return
                        new IRAPError()
                        {
                            ErrCode = 0,
                            ErrText = "更新成功",
                        };
                }
                catch (Exception error)
                {
                    string msg = $"更新内容时发生错误: {error.Message}";

                    Console.WriteLine(msg);
                    return
                        new IRAPError()
                        {
                            ErrCode = 9999,
                            ErrText = msg,
                        };
                }
            }
            else
            {
                string msg = $"传入的参数不是 BizNameSpaceEntity 对象";

                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }
        }

        public IRAPError Delete(int nameID, byte languageID = 30)
        {
            return
                Delete(
                    new BizNameSpaceEntity()
                    {
                        NameID = nameID,
                        LanguageID = languageID,
                    });
        }

        public IRAPError Delete(NameSpaceEntity src)
        {
            Repository<BizNameSpaceEntity> names = null;
            try
            {
                names = GetRepository();
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }

            if (src is BizNameSpaceEntity)
            {
                try
                {
                    var entity =
                        names.Entities.Find(
                            src.PartitioningKey,
                            src.LanguageID,
                            src.NameID);
                    if (entity == null)
                    {
                        return
                            new IRAPError()
                            {
                                ErrCode = 9999,
                                ErrText =
                                    $"未找到 PartitioningKey={src.PartitioningKey}|" +
                                    $"LanguagID={src.LanguageID}|" +
                                    $"NameID={src.NameID}的记录",
                            };
                    }

                    names.Delete(entity, false);
                    names.SaveChanges();

                    return
                        new IRAPError()
                        {
                            ErrCode = 0,
                            ErrText = "删除成功",
                        };
                }
                catch (Exception error)
                {
                    string msg = $"删除名称时发生错误: {error.Message}";

                    Console.WriteLine(msg);
                    return
                        new IRAPError()
                        {
                            ErrCode = 9999,
                            ErrText = msg,
                        };
                }
            }
            else
            {
                string msg = $"传入的参数不是 BizNameSpaceEntity 对象";

                Console.WriteLine(msg);
                return
                    new IRAPError()
                    {
                        ErrCode = 9999,
                        ErrText = msg,
                    };
            }
        }

        public NameSpaceEntity Get(int nameID, byte languageID = 30)
        {
            IQueryable<NameSpaceEntity> queryable = null;
            try
            {
                Repository<BizNameSpaceEntity> names = GetRepository();
                queryable = names.Table;
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return null;
            }

            var rlt =
                    queryable
                        .Where(
                            p => p.PartitioningKey == 0 &&
                                p.NameID == nameID &&
                                p.LanguageID == languageID).FirstOrDefault();
            return rlt;
        }

        public List<NameSpaceEntity> Get(byte languageID = 30)
        {
            IQueryable<NameSpaceEntity> queryable = null;
            try
            {
                Repository<BizNameSpaceEntity> names = GetRepository();
                queryable = names.Table;
            }
            catch (Exception error)
            {
                string msg = $"获取资源库的时候发生错误: {error.Message}";
                Console.WriteLine(msg);
                return null;
            }

            var rlt =
                    queryable
                        .Where(
                            p => p.PartitioningKey == 0 &&
                                p.LanguageID == languageID)
                        .OrderBy(p => p.NameID)
                        .ToList();

            return rlt;
        }
    }
}
