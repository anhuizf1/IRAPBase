 
using IRAPBase.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
 
       public class UnitOfWork : IUnitOfWork, IDisposable
       {
           public readonly IDbContext context;
           private bool disposed;
           private ConcurrentDictionary<string, object> repositories;

           public UnitOfWork(IDbContext context)
           {
               this.context = context;
           }

           public UnitOfWork()
           {

               context = new IRAPSqlDBContext();
           }

           public void Dispose()
           {
               Dispose(true);
               GC.SuppressFinalize(this);
           }

           public int Commit()
           {
              return context.SaveChanges();
           }

           public void RollBack()
           {
            context.RollBack();
           }

           public virtual void Dispose(bool disposing)
           {
               if (!disposed)
               {
                   if (disposing)
                   {
                       context.Dispose2();
                   }
               }
               disposed = true;
           }

           public Repository<T> Repository<T>() where T : BaseEntity
           {
               if (repositories == null)
               {
                   repositories = new ConcurrentDictionary<string, object>();
               }

               var type = typeof(T).Name;

               if (!repositories.ContainsKey(type))
               {
                  
                   var repositoryType = typeof(Repository<>);
                   var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);
                   repositories.TryAdd(type, repositoryInstance);
                 
               }

            return (Repository<T>)repositories[type];
           }
       }
    }
 
