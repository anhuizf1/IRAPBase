using IRAPBase.DTO;
using IRAPCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    /// <summary>
    ///  序列管理类：包括申请、新增、重置序列
    /// </summary>
    public class IRAPSequence
    {
        private string seqServerIPAddress = string.Empty;


        public IRAPSequence()
        {
            //seqServerIPAddress = "192.168.57.217";
            //读不到配置文件，所以下面注释掉
            // seqServerIPAddress = ConfigurationManager.AppSettings["SeqServer"].ToString();
            DllReadConfig readConfig = new DllReadConfig();
           if (readConfig.Parameters.ContainsKey("SeqServerAddr"))
            {
                seqServerIPAddress= readConfig.Parameters["SeqServerAddr"];
            } 
        }

        /// <summary>
        /// 获取交易号
        /// </summary>
        /// <returns>long</returns>
        public long GetTransactNo()
        {
            long transactNo = IRAPSeqClient.GetSequenceNo(seqServerIPAddress, "NextTransactNo", 1);
            return transactNo;
        }

        /// <summary>
        /// 获取登录标识（SysLogID）
        /// </summary>
        /// <returns>long</returns>

        public long GetSysLogID()
        {
            long sysLogID = IRAPSeqClient.GetSequenceNo(seqServerIPAddress, "NextSysLogID", 1);
            return sysLogID;
        }


        /// <summary>
        /// 通用获取序列号
        /// </summary>
        /// <param name="sequenceName">序列名称</param>
        /// <param name="cnt">申请数量</param>
        /// <returns>返回错误和序列值</returns>
        public SequenceValueDTO GetSequence(string sequenceName, int cnt)
        {
            SequenceValueDTO res = new SequenceValueDTO();
            if  (cnt<=0)
            {
                res.ErrCode = 99;
                res.ErrText = "申请数量不能为负数或零。";
                return res;
            }
            if (cnt > 10000)
            {
                res.ErrCode = 98;
                res.ErrText = "申请数量不能太大。";
                return res;
            }

            try
            {
                long sequenceValue = IRAPSeqClient.GetSequenceNo(seqServerIPAddress, sequenceName, cnt);
                res.ErrCode = 0;
                res.ErrText = "申请成功！";
                res.SequenceValue = sequenceValue;
                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"申请序列发生错误:{err.Message}";
                return res;
            } 
        }



        //重置序列
        public SequenceValueDTO ResetSequence(string sequenceName, long startValue)
        { 
            SequenceValueDTO res = new SequenceValueDTO();
            if (sequenceName == "")
            {
                res.ErrCode = 99;
                res.ErrText = "无效的重置序列";
                return res;
            }                                                                          

            try
            {
                long sequenceValue = IRAPSeqClient.ResetSequence(seqServerIPAddress, sequenceName, startValue);
                res.ErrCode = 0;
                res.ErrText = "重置序列成功！";
                res.SequenceValue = sequenceValue;
                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"重置序列发生错误:{err.Message}";
                return res;
            } 
        }

        //在线新增序列
        public SequenceValueDTO AddSequence (string sequenceName, long initValue)
        {
            SequenceValueDTO res = new SequenceValueDTO();
            if (sequenceName == "")
            {
                res.ErrCode = 99;
                res.ErrText = "无效的重置序列";
                return res;
            }

            try
            {
                long sequenceValue = IRAPSeqClient.AddSequence(seqServerIPAddress, sequenceName, initValue);
                res.ErrCode = 0;
                res.ErrText = "增加序列成功！";
                res.SequenceValue = sequenceValue;
                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"增加序列发生错误:{err.Message}";
                return res;
            }
        }
    }
}
