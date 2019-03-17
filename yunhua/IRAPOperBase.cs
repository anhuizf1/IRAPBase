using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using System.Data.Entity;

namespace IRAPBase
{

    /// <summary>
    /// 业务操作基础类
    /// </summary>
    public class IRAPOperBase
    {
        private int _opID = 0;
        private string _dbName;
        private string access_token = string.Empty;
        private int _communityID = 0;
        protected IDbContext _db = null;

        /// <summary>
        /// 根据实体类型获取存储对象DbSet,以便操作实体（增删改查）
        /// </summary>
        /// <param name="t">实体</param>
        /// <returns>表集合DbSet对象</returns>
        protected DbSet TableSet(BaseEntity t)
        {
            return _db.GetSet(t.GetType());
        }
        /// <summary>
        /// 社区标识
        /// </summary>
        public int CommunityID { get { return _communityID; } }

        /// <summary>
        /// 交易日志、辅助交易表分区键值
        /// </summary>
        public long TransPK
        {
            get { return (Int64)DateTime.Now.Year * 100000000L + (Int64)_communityID; }
        }

        /// <summary>
        /// 临时事实、固化事实、作废事实分区键值，以及辅助事实FactPartitioningKey
        /// </summary>
        public long FactPK {

            get { return  (Int64)DateTime.Now.Year * 1000000000000L + (Int64)_communityID * 10000L + (Int64)_opID; }
        }

        /// <summary>
        /// 行集事实分区键值
        /// </summary>
        public long RSFactPK
        {
            get {   return (Int64)DateTime.Now.Year * 1000000000000L + (Int64)_communityID * 10000L + (Int64)_opID; }
        }

        /// <summary>
        /// 辅助事实表中PartitioningKey字段
        /// </summary>
        /// <param name="dimLeafID"></param>
        /// <returns></returns>
        public long AuxFactPK(int dimLeafID)
        {
            return (Int64)_communityID * 1000000000000L + (Int64)dimLeafID * 10000L + (Int64)DateTime.Now.Year;
        }
        /// <summary>
        /// 开启业务操作的基础
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="access_token"></param>
        /// <param name="opID"></param>
        public IRAPOperBase(string dbName, string access_token, int opID)
        {
            _dbName = dbName;
            _opID = opID;
            _db = DBContextFactory.Instance.CreateContext(_dbName + "Context");
            this.access_token = access_token;
            _communityID = new IRAPLog().GetCommunityID(access_token);
            if (_communityID == 0)
            {
                throw new Exception($"令牌access_token={access_token}无效！");
            }
        }
        /// <summary>
        /// 申请交易号
        /// </summary>
        /// <param name="access_token">登录令牌</param>
        /// <param name="cnt">申请数量</param>
        /// <param name="remark">交易备注</param>
        /// <param name="voucherNo">票据号</param>
        /// <returns></returns>
        public long GetTransactNo(int cnt = 1, string remark = "", string voucherNo = "")
        {
            try
            {
                LoginEntity log = new IRAPLog().GetLogIDByToken(access_token);
                if (log == null)
                {
                    throw new Exception("申请交易号时出错，令牌无效！");
                }
                long transactNo = IRAPSequence.GetTransactNo();
                TransactEntity e = new TransactEntity()
                {
                    AgencyLeaf1 = log.AgencyLeaf,
                    IPAddress = log.IPAddress,
                    Operator = log.UserCode,
                    OperTime = DateTime.Now,
                    OpNodes = _opID.ToString(),
                    PartitioningKey = TransPK,
                    StationID = log.StationID,
                    Status = 1,
                    Remark = remark,
                    TransactNo = transactNo,
                    VoucherNo = voucherNo
                };
                _db.Set<TransactEntity>().Add(e);
                _db.SaveChanges();
                return transactNo;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        //获取操作类型
        //保存临时主事实
        public virtual IRAPError SaveTempFact(FactEntity e)
        {
            e.PartitioningKey = FactPK;
            TableSet(e).Add(e);
            _db.SaveChanges();
            return new IRAPError(0, "保存成功！");
        }
        public virtual IRAPError FixedFact(long  factID)
        {
            throw new NotImplementedException();
        }
        //保存辅助事实
        public virtual IRAPError SaveAuxFact(BaseAuxFact e)
        {
            e.PartitioningKey = AuxFactPK(0);
            e.FactPartitioningKey = FactPK;
            TableSet(e).Add(e);
            _db.SaveChanges();
            return new IRAPError(0, "辅助保存成功！");
        }
        //保存行集事实

        public virtual IRAPError SaveRSFact(List<BaseRSFact> list)
        {
            foreach (BaseRSFact r in list)
            {
                r.PartitioningKey = RSFactPK;
                TableSet(r).Add(r);
            }
            _db.SaveChanges();
            return new IRAPError(0, "行集保存成功！");
        }


        //查询主事实
        //查询辅助事实
        //查询行集事实


    }
}
