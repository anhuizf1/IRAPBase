using IRAPBase.DTO;
using IRAPCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPSequence
    {
        private string seqServerIPAddress = string.Empty;


        public IRAPSequence()
        {
            seqServerIPAddress= ConfigurationManager.AppSettings["SeqServer"].ToString();
        }


        //获取登录标识
        public long GetTransactNo()
        {
            long transactNo = IRAPSeqClient.GetSequenceNo(seqServerIPAddress, "NextTransactNo", 1);
            return transactNo;
        }

        //获取登录标识
        public  long GetSysLogID()
        {
            long sysLogID = IRAPSeqClient.GetSequenceNo(seqServerIPAddress, "NextSysLogID", 1);
            return sysLogID;
        }

        //通用获取序列号

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
            throw new NotImplementedException();
        }

        //在线新增序列
        public SequenceValueDTO AddSequence (string sequenceName, long initValue)
        {
            throw new NotImplementedException();
        }
    }
}
