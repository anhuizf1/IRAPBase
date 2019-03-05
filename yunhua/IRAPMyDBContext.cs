using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using IRAPBase.Entities;
using System.Linq.Expressions;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using MySql.Data.EntityFramework;
using System.Collections;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace IRAPBase
{
    //基本树

    //禁止代码迁移
 
    //public class MyConfiguration : DbMigrationsConfiguration<IRAPDBContext>
    //{
    //    public MyConfiguration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //    }
    //}
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class IRAPMyDBContext : DbContext,IDbContext
    {
        public bool AutoDetectChangesEnabled
        {
            get { return Configuration.AutoDetectChangesEnabled; }
            set { Configuration.AutoDetectChangesEnabled = value; }
        }
        public Database DataBase { get { return this.Database; } }

        public ObjectContext GetObjectContext { get { return ((IObjectContextAdapter)this).ObjectContext; } }
        public IRAPMyDBContext()
            : base("name=IRAPMyDBContext")
        {
            this.Database.Log = message => Console.WriteLine(message);
            //生成简单的SQL
            Configuration.UseDatabaseNullSemantics = true;
            //不生成数据库
            Database.SetInitializer<IRAPMyDBContext>(null);
            //  Database.SetInitializer<IRAPDBContext>(new DropCreateDatabaseIfModelChanges<IRAPDBContext>());
        }
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
            Configuration.LazyLoadingEnabled = true;
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

        public new DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return base.Entry(entity);
        }

       

        public override int SaveChanges()
        {
            return SaveChanges();
            
        }

        public void RollBack()
        {
            Database.CurrentTransaction.Rollback();
        }
        public void Dispose2()
        {
            base.Dispose();
        }
    }

     


}