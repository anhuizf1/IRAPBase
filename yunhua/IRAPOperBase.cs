using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using System.Data.Entity;
using IRAPBase.Serialize;

namespace IRAPBase
{

    /// <summary>
    /// 业务操作基础类
    /// </summary>
    public class IRAPOperBase
    {
        private int _opID = 0;
  
        private string access_token = string.Empty;
        private int _communityID = 0;
        protected IDbContext _db = null;
        private LoginEntity log = null;
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
        private int CommunityID { get { return _communityID; } }

        /// <summary>
        /// 交易日志、辅助交易表分区键值
        /// </summary>
        private long TransPK
        {
            get { return (Int64)DateTime.Now.Year * 100000000L + (Int64)_communityID; }
        }

        /// <summary>
        /// 临时事实、固化事实、作废事实分区键值，以及辅助事实FactPartitioningKey
        /// </summary>
        private long FactPK {

            get { return  (Int64)DateTime.Now.Year * 1000000000000L + (Int64)_communityID * 10000L + (Int64)_opID; }
        }

        /// <summary>
        /// 行集事实分区键值
        /// </summary>
        private long RSFactPK
        {
            get {   return (Int64)DateTime.Now.Year * 1000000000000L + (Int64)_communityID * 10000L + (Int64)_opID; }
        }

        /// <summary>
        /// 辅助事实表中PartitioningKey字段
        /// </summary>
        /// <param name="dimLeafID"></param>
        /// <returns></returns>
        private long AuxFactPK(int dimLeafID)
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
            _opID = opID;
            _db = DBContextFactory.Instance.CreateContext(dbName + "Context");
            this.access_token = access_token;
            var logDB = new IRAPLog();
            _communityID = logDB.GetCommunityID(access_token);
            if (_communityID == 0)
            {
                throw new Exception($"令牌access_token={access_token}无效！");
            }
            log = logDB.GetLogIDByToken(access_token);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="access_token">登录令牌</param>
        /// <param name="opID">业务操作</param>
        public IRAPOperBase(IDbContext db, string access_token, int opID)
        {
            _db = db;
            _opID = opID;
            this.access_token = access_token;
            var logDB = new IRAPLog();
            _communityID = logDB.GetCommunityID(access_token);
            if (_communityID == 0)
            {
                throw new Exception($"令牌access_token={access_token}无效！");
            }
            log = logDB.GetLogIDByToken(access_token);
        }

        /// <summary>
        /// 申请事实编号，如有错误会抛出异常，请捕捉异常
        /// </summary>
        /// <param name="cnt">申请数量</param>
        /// <returns>事实编号</returns>
        public long GetFactNo (int cnt = 1)
        {
           var resDTO= IRAPSequence.GetSequence("NextFactNo", cnt);
            if (resDTO.ErrCode != 0)
            {
                throw new Exception(resDTO.ErrText);
            }
            return resDTO.SequenceValue;
        }
        /// <summary>
        /// 申请交易号
        /// </summary>
        /// <param name="cnt">申请数量</param>
        /// <param name="remark">交易备注</param>
        /// <param name="opNodes">操作类型清单,多个用逗号隔开</param>
        /// <param name="voucherNo">票据号</param>
        /// <returns></returns>
        public long GetTransactNo(int cnt = 1, string remark = "", string opNodes="", string voucherNo = "")
        {
            try
            {
                if (opNodes == "")
                {
                    opNodes = (-_opID).ToString();
                }
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
        /// <summary>
        /// 保存临时主事实
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual IRAPError SaveTempFact(FactEntity e)
        {
            if (e.FactID <= 0)
            {
                return new IRAPError(22, "事实编号不能为小于等于0！");
            }
            if (e.OpType == 0)
            {
                return new IRAPError(22, "操作类型不能为0，没有操作类型时请传1 ！");
            }
            e.OpID = _opID;
            e.BusinessDate = DateTime.Now;
            e.PartitioningKey = FactPK;
            TableSet(e).Add(e);
           // _db.SaveChanges();
            return new IRAPError(0, "保存成功！");
        }
        /// <summary>
        /// 固化事实
        /// </summary>
        /// <param name="factID">事实编号</param>
        /// <returns></returns>
        public virtual IRAPError FixedFact(long  factID)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 保存辅助事实
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual IRAPError SaveAuxFact(BaseAuxFact e)
        {
            e.PartitioningKey = AuxFactPK(0);
            e.FactPartitioningKey = FactPK;
            TableSet(e).Add(e);
          //  _db.SaveChanges();
            return new IRAPError(0, "辅助保存成功！");
        }
     
        /// <summary>
        /// 保存行集事实
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual IRAPError SaveRSFact(List<BaseRSFact> list)
        {
            foreach (BaseRSFact r in list)
            {
                r.PartitioningKey = RSFactPK;
                TableSet(r).Add(r);
            }
           // _db.SaveChanges();
            return new IRAPError(0, "行集保存成功！");
        }
        /// <summary>
        /// 保存UTB或log表（单条记录）
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual IRAPError SaveUTBOrLog(BaseEntity e)
        {
            TableSet(e).Add(e);
            //  _db.SaveChanges();
            return new IRAPError(0, "日志数据保存成功！");
        }
        /// <summary>
        /// 保存UTB或log表（多条记录）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual IRAPError SaveUTBOrLog(List<BaseEntity> list)
        {
            foreach (BaseEntity r in list)
            {      
                TableSet(r).Add(r);
            }
            //  _db.SaveChanges();
            return new IRAPError(0, "日志数据保存成功！");
        }

        /// <summary>
        /// 撤销交易
        /// </summary>
        /// <param name="transactNo">交易号</param>
        /// <param name="deleteFact">是否删除事实(默认是)</param>
        /// <returns></returns>
        public virtual IRAPError UnCheckTransact(long transactNo, bool deleteFact=true)
        {

            TransactEntity t1 = GetEntities<TransactEntity>().FirstOrDefault(c => c.PartitioningKey == TransPK && c.TransactNo == transactNo);

            if (t1 == null)
            {
                throw new Exception($"交易号：{t1.TransactNo} 不存在！");
            }
            t1.Status = 1;
            t1.Checked = "";
            t1.OkayTime = null;
            if (deleteFact)
            {
                IDbSet<FactEntity> tempFactList = _db.Set<FactEntity>();
                var list=  tempFactList.Where(c => c.TransactNo == t1.TransactNo && c.PartitioningKey== FactPK);
                foreach(FactEntity r in list)
                {
                    tempFactList.Remove(r);
                }
            }
            SaveChanges();
            return new IRAPError(0, "交易撤销成功！");
        }


        /// <summary>
        /// 复核交易
        /// </summary>
        /// <param name="transactNo"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public IRAPError CheckTransact(long transactNo)
        {
            TransactEntity t1 = GetEntities<TransactEntity>().FirstOrDefault(c => c.PartitioningKey == TransPK && c.TransactNo == transactNo);

            if (t1 == null)
            {
                throw new Exception($"交易号：{t1.TransactNo} 不存在！");
            }
            t1.Checked = log.UserCode;
            t1.OkayTime = DateTime.Now;
            t1.Status = 3;
            SaveChanges();
            return new IRAPError(0,"交易复核成功！");
        }
        /// <summary>
        /// 交易撤销到回收站
        /// </summary>
        /// <param name="transactNo"></param>
        /// <returns></returns>
        public IRAPError DeleteToRecycle(long transactNo)
        {
            TransactEntity t1 = GetEntities<TransactEntity>().FirstOrDefault(c => c.PartitioningKey == TransPK && c.TransactNo == transactNo);

            if (t1 == null)
            {
                throw new Exception($"交易号：{t1.TransactNo} 不存在！");
            }
            if (t1.Status>3)
            {
                throw new Exception($"交易号：{t1.TransactNo} 已被撤销或已被固化！{t1.Status}");
            }
            t1.Revoker = log.UserCode;
            t1.RevokeTime = DateTime.Now;
            t1.Status = 4;
            //移动到Recycle
           var list=  GetEntities<FactEntity>().Where(c => c.PartitioningKey == FactPK && c.TransactNo == transactNo);

            int i = 0;
            foreach(var r in list)
            {
                RecycleFactEntity t2 = new RecycleFactEntity();
                r.CopyTo(t2);
                t2.Remark = t2.Remark + "[撤销]";
                _db.Set<RecycleFactEntity>().Add(t2);
                _db.Set<FactEntity>().Remove(r);
                i++;
            }
            if (i==0)
            {
                return new IRAPError(11, "交易没有对应的事实记录！");
            }
            SaveChanges();
            return new IRAPError(0, "交易撤销成功！");
        }
        /// <summary>
        /// 删除主事实
        /// </summary>
        /// <param name="factID"></param>
        /// <returns></returns>
        public IRAPError DeleteToRecycleByFactID(long factID)
        {
             
            //移动到Recycle
            var list = GetEntities<FactEntity>().Where(c => c.PartitioningKey == FactPK && c.FactID == factID);
            int i = 0;
            foreach (var r in list)
            {
                RecycleFactEntity t2 = new RecycleFactEntity();
                r.CopyTo(t2);
                t2.Remark = t2.Remark + "[删除]";
                _db.Set<RecycleFactEntity>().Add(t2);
                _db.Set<FactEntity>().Remove(r);
                i++;
            }
            if (i == 0)
            {
                return new IRAPError(11, "交易没有对应的事实记录！");
            }
            SaveChanges();
            return new IRAPError(0, "事实删除成功！");
        }
        /// <summary>
        /// 提交变更到数据库
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return _db.SaveChanges();
        }

        /// <summary>
        /// 开启一个新事务
        /// </summary>
        public void BeginTransaction()
        {
            if (_db.DataBase.CurrentTransaction != null)
            {
                _db.DataBase.CurrentTransaction.Rollback();
                _db.DataBase.CurrentTransaction.Dispose();
            }
            _db.DataBase.BeginTransaction();
        }

        /// <summary>
        /// 对默认数据库连接进行提交
        /// </summary>
        public void Commit()
        {
            _db.SaveChanges();
            if (_db.DataBase.CurrentTransaction != null)
            {
                _db.DataBase.CurrentTransaction.Commit();
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack() {

            if (_db.DataBase.CurrentTransaction != null)
            {
                _db.DataBase.CurrentTransaction.Rollback();
                _db.DataBase.CurrentTransaction.Dispose();
            }
        }
        /// <summary>
        /// 查询所有表
        /// </summary>
        /// <typeparam name="T">要查询的实体类型</typeparam>
        /// <returns></returns>
        public IQueryable<T> GetEntities<T>() where T : BaseEntity
        {
            return _db.Set<T>();
        }

        /// <summary>
        /// 查询交易实体（最近两年）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="communityID"></param>
        /// <param name="transactNo"></param>
        /// <returns></returns>
        public static  TransactEntity GetTransact(IDbContext db,int communityID, long transactNo)
        {
            long[] dict = new long[] { GetTransPK(communityID, DateTime.Now.Year), GetTransPK(communityID, DateTime.Now.Year - 1) };
            return   db.Set<TransactEntity>().FirstOrDefault(c => dict.Contains(c.PartitioningKey) && c.TransactNo == transactNo);
        }

        /// <summary>
        /// 查询交易实体清单，外面再根据其他条件过滤(限定最近两年)
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="communityID">社区</param>
        /// <returns></returns>
        public static IQueryable<TransactEntity> GetTransactList(IDbContext db ,int communityID)
        {
            long[] dict = new long[] { GetTransPK(communityID, DateTime.Now.Year), GetTransPK(communityID, DateTime.Now.Year - 1) };
            return db.Set<TransactEntity>().Where(c => dict.Contains(c.PartitioningKey)  );
        }
        //查询辅助事实
        //查询行集事实

        /// <summary>
        /// 计算PK值
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="treeID">树标识</param>
        /// <returns></returns>
        public static long PartitioningKey(int communityID , int treeID)
        {
            return communityID * 10000L + treeID;
        }
        /// <summary>
        /// 交易日志、辅助交易表分区键值
        /// </summary>
        /// <param name="communityID"></param>
        /// <param name="yearID"></param>
        /// <returns></returns>
        public static long GetTransPK(int communityID , int yearID)
        {
            return (Int64)yearID * 100000000L + (Int64)communityID;  
        }

        /// <summary>
        /// 临时事实、固化事实、作废事实分区键值，以及辅助事实FactPartitioningKey
        /// </summary>
        public static long GetFactPK(int communityID,int yearID,int opID )
        {
            return (Int64)yearID * 1000000000000L + (Int64)communityID * 10000L + (Int64)opID; 
        }
        /// <summary>
        /// 行集事实分区键值
        /// </summary>
        public static long GetRSFactPK(int communityID , int yearID ,int opID)
        {
            return (Int64)yearID * 1000000000000L + (Int64)communityID * 10000L + (Int64)opID;  
        }
        /// <summary>
        /// 辅助事实表中PartitioningKey字段
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="yearID">4位年份</param>
        /// <param name="dimLeafID">维度标识</param>
        /// <returns></returns>
        public static long GetAuxFactPK(int communityID, int yearID, int dimLeafID)
        {
            return (Int64)communityID * 1000000000000L + (Int64)dimLeafID * 10000L + (Int64)yearID;
        }
    }
}
