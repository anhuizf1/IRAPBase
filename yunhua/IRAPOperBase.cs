using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPBase
{

    /// <summary>
    /// 业务操作基础类
    /// </summary>
    public class IRAPOperBase
    {
        private int _opID = 0;
        private IDbContext _db;
        public IRAPOperBase(IDbContext  db , int opID)
        {
            db = _db;
            _opID = opID;
        }

        //获取操作类型


        //保存临时主事实
        public IRAPError SaveTempFact(FactEntity e)
        {
            return null;
        }
        public IRAPError SaveFixedFact(FactEntity e)
        {
            return null;
        }
        //保存辅助事实
        public IRAPError SaveAuxFact<T>(T t)
        {
            return null;
        }
        //保存行集事实

        public IRAPError SaveRSFact<T>(List<T> list)
        {
            return null;
        }
      


        //查询主事实
        //查询辅助事实
        //查询行集事实
        

    }
}
