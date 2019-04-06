using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IRAPBase
{
    /// <summary>
    /// 实现树的个性化部分接口（个性化的树继承此接口）作废不用了
    /// </summary>
   /*
    *public interface ITreeBase
    {
        IQueryable<BaseRowAttrEntity> GetRowAttr(int ordinal);
        IQueryable<BaseGenAttrEntity> GetGenAttr();

        IRAPError SaveGenAttr(BaseGenAttrEntity e);

        IRAPError SaveRSAttr(List<BaseRowAttrEntity> list);
    }

    
    public class IRAPTree1 : ITreeBase
    {

        private IDbContext _db = null;
        public IRAPTree1(IDbContext db)
        {
            _db = db;
        }
        public  IQueryable<BaseRowAttrEntity> GetRowAttr(int ordinal)
        {

            Repository<RowSet_T1R1> dbRepo = new Repository<RowSet_T1R1>(_db);
            return dbRepo.Table;
        }
        public IQueryable<BaseGenAttrEntity> GetGenAttr()
        {
            Repository<GenAttr_T1> dbRepo = new Repository<GenAttr_T1>(_db);
            return dbRepo.Table;
        }

        public IRAPError SaveGenAttr(BaseGenAttrEntity e)
        {
            IRAPError error = new IRAPError();
            Repository<GenAttr_T1> dbRepo = new Repository<GenAttr_T1>(_db);
            GenAttr_T1 genEntity= dbRepo.Table.FirstOrDefault(r => r.PartitioningKey == e.PartitioningKey && r.EntityID == e.EntityID);
            if (genEntity == null)
            {
                GenAttr_T1 t1Entity = e as GenAttr_T1;
                dbRepo.Insert(t1Entity);
            }
            else
            {
                dbRepo.Update(genEntity);
            }
            int res= dbRepo.SaveChanges();
            if (res > 0)
            {
                error.ErrCode = 0;
                error.ErrText = "更新成功！";
            }
            else
            {
                error.ErrCode = 22;
                error.ErrText = "没有更新成功！";
            }
            return error;
        }

        public IRAPError SaveRSAttr(List<BaseRowAttrEntity> list)
        {
            IRAPError error = new IRAPError(0,"保存成功！");
            try
            {
                if ( list.Count==0)
                {
                    error.ErrCode = 9;
                    error.ErrText = "传入集合无数据，无需保存！";
                    return error;
                }
                long pk = list[0].PartitioningKey;
                int entityID = list[0].EntityID;
                Repository<RowSet_T1R1> dbRepo = new Repository<RowSet_T1R1>(_db);
                var toDelete=  dbRepo.Table.Where(r => r.PartitioningKey == pk & r.EntityID == entityID).ToList();
                foreach(RowSet_T1R1 r in toDelete)
                {
                    dbRepo.Delete(r);
                }
                foreach (RowSet_T1R1 r in list)
                {
                    dbRepo.Insert(r);
                }
                dbRepo.SaveChanges();
                return error;
            }catch(Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"保存行集属性发生异常：{err.Message}";
                return error;
            }
        }
    } */
}
