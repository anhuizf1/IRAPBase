/*----------------------------------------------------------------
// Copyright © 2019 Chinairap.All rights reserved. 
// CLR版本：	4.0.30319.42000
// 类 名 称：    RequestHelper
// 文 件 名：    RequestHelper
// 创建者：      DUWENINK
// 创建日期：	2019/7/17 17:17:43
// 版本	日期					修改人	
// v0.1	2019/7/17 17:17:43	DUWENINK
//----------------------------------------------------------------*/
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPCommon;
using Newtonsoft.Json;
using System.Net;

namespace IRAPBase.ServiceDiscovery
{
    /// <summary>
    /// 命名空间： IRAPBase.ServiceDiscovery
    /// 创建者：   DUWENINK
    /// 创建日期： 2019/7/17 17:17:43
    /// 类名：     RequestHelper
    /// http请求发起类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="RequestTResult"></typeparam>
        /// <param name="getRequest"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<T> ResponseAsync<T, RequestTResult>(RequestTResult getRequest, Method method = Method.GET) where RequestTResult : RequestResult
        {

            var request = new RestRequest(method);
            var client = new RestClient(getRequest.Url);
            //if (getRequest.Data != null) {
            //    request.AddParameter("undefined", JsonConvert.SerializeObject(getRequest),ParameterType.RequestBody);
            //}
            request.AddHeader("cache-control", "no-cache");
            var response = await client.ExecuteTaskAsync(request);
            return JsonConvert.DeserializeObject<T>(response.StatusCode == HttpStatusCode.OK ? response.Content : null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="RequestTResult"></typeparam>
        /// <param name="getRequest"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static  T Response<T, RequestTResult>(RequestTResult getRequest, Method method = Method.GET) where RequestTResult : RequestResult
        {
            return ResponseAsync<T, RequestTResult>(getRequest, method).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="RequestTResult"></typeparam>
        /// <param name="getRequest"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<T> ResponseTAsync<T, K, RequestTResult>(RequestTResult getRequest, Method method = Method.GET) where RequestTResult : RequestResult<K>
        {

            var request = new RestRequest(method);
            var client = new RestClient(getRequest.Url);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            if (getRequest.Data != null)
            {
                request.AddParameter("undefined", JsonConvert.SerializeObject(getRequest.Data), ParameterType.RequestBody);
            }

            var response = await client.ExecuteTaskAsync(request);
            return JsonConvert.DeserializeObject<T>(response.StatusCode == HttpStatusCode.OK ? response.Content : null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="RequestTResult"></typeparam>
        /// <param name="getRequest"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T ResponseT<T, K, RequestTResult>(RequestTResult getRequest, Method method = Method.GET) where RequestTResult : RequestResult<K>
        {

            return ResponseTAsync<T, K, RequestTResult>(getRequest, method).Result;
        }




    }
}
