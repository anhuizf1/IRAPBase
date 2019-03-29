using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using IRAPBase.Entities;
using System.Data.Entity.Migrations;
using System.Collections;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace IRAPBase
{
    public class MyConfiguration : DbMigrationsConfiguration<IRAPSqlDBContext>
    {
        public MyConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
    /// <summary>
    /// SQLServer专用连接上下文类
    /// </summary>
    public class IRAPSqlDBContext : DbContext, IDbContext
    {
        // private Assembly extendAssembly = null;
        //public static DbModelBuilder _modelBuilder = null;

          
        public bool AutoDetectChangesEnabled
        {
            get { return Configuration.AutoDetectChangesEnabled; }
            set { Configuration.AutoDetectChangesEnabled = value; }
        }
        /// <summary>
        /// 数据库
        /// </summary>
        public Database DataBase { get { return this.Database; } }
        /// <summary>
        /// 上下文
        /// </summary>
        public ObjectContext GetObjectContext { get { return ((IObjectContextAdapter)this).ObjectContext; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnectionStr">数据库连接，与配置文件中的key值对应</param>
        public IRAPSqlDBContext(string dbConnectionStr) : base($"name={dbConnectionStr}")
        //  public IRAPSqlDBContext() //: base("name=IRAPSqlDBContext")                    
        {
            this.Database.Log = message => Console.Write(message);
            //生成简单的SQL
            Configuration.UseDatabaseNullSemantics = true;
            //不生成数据库
            Database.SetInitializer<IRAPSqlDBContext>(null);
            //  Database.SetInitializer<IRAPDBContext>(new DropCreateDatabaseIfModelChanges<IRAPDBContext>());
        }
        /// <summary>
        /// 创建数据库连接默认IRAPSqlDB这个连接
        /// </summary>
        public IRAPSqlDBContext() //: base("name=IRAPSqlDBContext")                    
        {
            this.Database.Log = message => Console.WriteLine(message);

            //生成简单的SQL
            Configuration.UseDatabaseNullSemantics = true;
            //不生成数据库
            Database.SetInitializer<IRAPSqlDBContext>(null);
            //  Database.SetInitializer<IRAPDBContext>(new DropCreateDatabaseIfModelChanges<IRAPDBContext>());
        }

        /// <summary>
        /// 模型创建时设置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //自动执行配置
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            Configuration.LazyLoadingEnabled = true;

            //从配置文件中预先加载外部程序集
            Hashtable loadassemblyList = (Hashtable)ConfigurationManager.GetSection("LoadAssembly");
            foreach (string key in loadassemblyList.Keys)
            {
                var extendTypesToRegister = Assembly.Load(key).GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace))
                    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
                foreach (var type in extendTypesToRegister)
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);

                    modelBuilder.Configurations.Add(configurationInstance);
                }
            }
            //设置数据库null值
            Configuration.UseDatabaseNullSemantics = true;
            // modelBuilder.Configurations.Add( );
            // modelBuilder.Properties().Where(p => p.Name == "LeafID").Configure(p => p.IsKey());
            // modelBuilder.Properties<string>().Configure(c => c.HasMaxLength(500));
            //使用类继承，EF会自动生成复数的表，解决办法是
            //  modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //decimal精度保留2位
            //  modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(15, 2));
            //modelBuilder.Entity<InterFace>().ToTable("stb_Interface");
            //  modelBuilder.Entity<SysTreeDir>().HasKey(k => new { PartitioningKey = k.PartitioningKey, TreeID = k.TreeID, LeafID = k.LeafID });
            //  modelBuilder.Entity<BizTreeDir>().HasKey(k => new { PartitioningKey = k.PartitioningKey, TreeID = k.TreeID, LeafID = k.LeafID });
            // modelBuilder.Entity<RichTreeDir>().HasKey(k => new { PartitioningKey = k.PartitioningKey, TreeID = k.TreeID, LeafID = k.LeafID });

            //  modelBuilder.Entity<BizTreeClassfiy>().HasKey(k => new { PartitioningKey = k.PartitioningKey, TreeID = k.TreeID, LeafID = k.LeafID });
            //modelBuilder.Entity<InterFace>().Property(p => p.Prefix).IsRequired();
            ////设置列的最大长度
            //modelBuilder.Entity<InterFace>().Property(p => p.Prefix).HasColumnType("VARCHAR").HasMaxLength(60);
            //modelBuilder.Entity<TreeBase>().ToTable("stb058");
            //modelBuilder.Entity<TreeBase>().HasKey(k => k.LeafID);
            // modelBuilder.Entity<ModelTreeClassfiy>();
            base.OnModelCreating(modelBuilder);
        }
        //注册实体类
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

       /// <summary>
       /// 根据类型返回DbSet
       /// </summary>
       /// <param name="t"></param>
       /// <returns></returns>
        public DbSet GetSet(Type t)
        {
            return Set(t);
        }
        public new DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : BaseEntity
        {

            return base.Entry(entity);
        }

        /// <summary>
        /// 保存数据库变化
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {

            return base.SaveChanges();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            if (base.Database.CurrentTransaction != null)
            {
                base.Database.CurrentTransaction.Rollback();
            }
        }
        [Obsolete]
        public void Dispose2()
        {
            base.Dispose();
        }
    }


}
