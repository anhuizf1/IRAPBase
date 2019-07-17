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
using System;
using System.Runtime.Serialization;

namespace IRAPBase.ServiceDiscovery
{
    [Serializable]
    internal class ExCodeNotFoundException : Exception
    {
        public ExCodeNotFoundException()
        {
        }

        public ExCodeNotFoundException(string message) : base(message)
        {
        }

        public ExCodeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExCodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}