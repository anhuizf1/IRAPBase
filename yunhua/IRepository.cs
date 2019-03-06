using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using IRAPBase.Entities;
namespace IRAPBase
{
    public interface IRepository<TEntity> where TEntity: BaseEntity
   {
        TEntity GetById(object id);
        #region 属性
       // IDbSet<TEntity> Entities { get; }
        #endregion

        #region 公共方法
        void Insert(TEntity entity);

     //   int Insert(IEnumerable entities);

       // int Delete(TEntity entity);

        void Delete(TEntity entity, bool isAttach = false);

        void Update(TEntity entity);

        IQueryable<TEntity> Table { get; }

        int SaveChanges();

       //void Attach(TEntity entity);
       //TEntity GetByKey(object key);
        #endregion
    }

}
