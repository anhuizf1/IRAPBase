using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
   public class EGrant : BaseEntity
    {
        //IRAP的权限表
            public long PartitioningKey { get; set; }
           [DatabaseGenerated(DatabaseGeneratedOption.None)]//不自动增长
           public int PermissionID { get; set; }
            public int AgencyNode { get; set; }
            public int RoleNode { get; set; }
            public string Scenarios { get; set; }
            public Int16 TreeID { get; set; }
            public int CSTRoot { get; set; }
            public string DicingFilter { get; set; }
            public string RSAttrFilter { get; set; }
            public bool IsValid { get; set; }
        
    }

    public class IRAPGrantMap : EntityTypeConfiguration<EGrant>
    {

        public IRAPGrantMap()
        {
            //表定义
            ToTable("stb020");
            HasKey(t => t.PermissionID);
            Property(t => t.PermissionID).HasDatabaseGeneratedOption(   DatabaseGeneratedOption.None);
            Property(t => t.CSTRoot).IsRequired();
            Property(t => t.AgencyNode).IsRequired();
            Property(t => t.RoleNode).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
