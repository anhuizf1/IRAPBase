using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace IRAPBase
{
    //做业务的基础类（这个类有争议）
    public class IRAPFuncBase
    {

        private IDbContext _dbContext = null;
        private int _t3LeafID = 0;
        Dictionary<int,  IRAPOperBase> _operList = null;
        
        public IRAPFuncBase(IDbContext db)
        {
            _operList = new Dictionary<int, IRAPOperBase>(); 
            _dbContext = db;  
        }
        public IRAPFuncBase(IDbContext db, int t3LeafID)
        {
            _dbContext = db;
            _t3LeafID = t3LeafID;
            _operList = new Dictionary<int, IRAPOperBase>();
        }
        //增加业务操作
        public void AddOperation(int opID)
        {
            //_operList.Add(opID, new IRAPOperBase(_dbContext, opID));
        }

        //申请序列号
        public virtual long GetTransactNo()
        {
            return 0;
        }

        //能否办理业务
        protected virtual IRAPError GetLimits(params string[] paramArray)
        {
            return new IRAPError(0,"可以办理业务！");
        }

        //保存主交易
        protected virtual IRAPError SaveTransact(TransactEntity e)
        {
            return null;
        }
        //保存辅助交易
        protected virtual IRAPError SaveAuxTran<T>( T t)
        {
            return null;
        }
        //保存个性化日志
        protected virtual IRAPError SaveUTBLog<T>( T t)
        {
            return null;
        }

        //继承子类必须重写这个方法实现逻辑
        public virtual IRAPError DoBiz(string jsonParam)
        {
            
            return null;
        }


  

    }
}
