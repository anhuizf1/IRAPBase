/*----------------------------------------------------------------
// Copyright © 2019 Chinairap.All rights reserved. 
// CLR版本：	4.0.30319.42000
// 类 名 称：    ServiceRPC
// 文 件 名：    ServiceRPC
// 创建者：      DUWENINK
// 创建日期：	2019/7/17 17:15:05
// 版本	日期					修改人	
// v0.1	2019/7/17 17:15:05	DUWENINK
//----------------------------------------------------------------*/
using IRAPBase.Exceptions;
using IRAPCommon.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.ServiceDiscovery
{
    /// <summary>
    /// 命名空间： IRAPBase.ServiceDiscovery
    /// 创建者：   DUWENINK
    /// 创建日期： 2019/7/17 17:15:05
    /// 类名：     ServiceRPC
    /// </summary>
    public class ServiceRPC
    {
        /// <summary>
        /// 业务前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 业务号
        /// </summary>
        public string ExCode { get; set; }


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="prefix">业务前缀</param>
        /// <param name="exCode">业务号</param>
        public ServiceRPC(string prefix, string exCode)
        {
            Prefix = prefix;
            ExCode = exCode;
            if (prefix.IsBlank())
            {
                throw new PrefixNotFoundException("业务前缀不能为Null");
            }
            if (prefix.Split('_').Length != 2)
            {
                throw new PrefixNotCompliantException("业务前缀不符合规范");
            }
            if (exCode.IsBlank())
            {
                throw new ExCodeNotFoundException("业务码不能为Null");
            }
        }




    }
}
