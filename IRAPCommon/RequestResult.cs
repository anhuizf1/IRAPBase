/*----------------------------------------------------------------
// Copyright © 2019 Chinairap.All rights reserved. 
// CLR版本：	4.0.30319.42000
// 类 名 称：    RequestResult
// 文 件 名：    RequestResult
// 创建者：      DUWENINK
// 创建日期：	2019/7/17 17:20:05
// 版本	日期					修改人	
// v0.1	2019/7/17 17:20:05	DUWENINK
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{
    /// <summary>
    /// 命名空间： IRAPCommon
    /// 创建者：   DUWENINK
    /// 创建日期： 2019/7/17 17:20:05
    /// 类名：     RequestResult
    /// </summary>
    public class RequestResult
    {
        public string Url { get; set; }
    }


    /// <summary>
    /// RequestResult泛型形式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestResult<T> : RequestResult
    {

        public RequestResult()
        {

        }
        public T Data { get; set; }
    }



}
