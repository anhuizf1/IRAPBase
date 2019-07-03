using IRAPBase.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data.Entity.Core.Objects;
using System.Reflection;

namespace IRAPBase
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext context;
       
        private IDbSet<T> entities;
        string errorMessage = string.Empty;
        public Repository(string context)
        {
            this.context = new IRAPSqlDBContext( context);
        }
        public Repository()
        {
            this.context = new IRAPSqlDBContext("IRAPContext");

        }
        public Repository(IDbContext context)
        {
            this.context = context;
        }
        public Database DataBase { get { return context.DataBase; } }
       
        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        //public void Attach(T entity)
        //{
        //    this.Entities.Attach(entity);
        //}
        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entities.Add(entity); 
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(errorMessage, dbEx);
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.context.Entry(entity).State = EntityState.Modified;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                throw new Exception(errorMessage, dbEx);
            }
        }

        public void Delete(T entity, bool isAttach=true )
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if(!isAttach)
                {
                   //  this.Entities.Attach(entity);
                    this.context.Entry(entity).State = EntityState.Deleted;
                } 
                this.Entities.Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception(errorMessage, dbEx);
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        public IDbSet<T> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = context.Set<T>();
                    
                }
                return entities;
            }
        }
        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
