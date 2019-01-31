using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using IRAPBase.Entities;

namespace IRAPBase
{
    public interface IDbContext
    {

        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : BaseEntity;

       // IDbSet<TEntity> AddSet<TEntity>() where TEntity : BaseEntity;
        int SaveChanges();
        void Dispose2();

        void RollBack();
    }
}
