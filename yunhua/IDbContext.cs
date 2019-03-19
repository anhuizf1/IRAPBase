using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using IRAPBase.Entities;
using System.Data.Entity.Core.Objects;

namespace IRAPBase
{
    public interface IDbContext
    {

        Database DataBase { get; }

        ObjectContext GetObjectContext { get; }
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        DbSet GetSet(Type t);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : BaseEntity;

       // IDbSet<TEntity> AddSet<TEntity>() where TEntity : BaseEntity;
        int SaveChanges();
        void Dispose2();

        void RollBack();
    }
}
