using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb012")]
    public  class SystemNodeEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        public int NodeID { get; set; }
        public string HotKey { get; set; }
        public string AccelerateKey { get; set; }
        public int MicroHelpNameID { get; set; }
        public int ToolBarItemHintNameID { get; set; }
        public int T291LeafID_ToolBarItemIcon { get; set; }
        public bool NewMenuGroup { get; set; }
        public bool ToolBarNewSpace { get; set; }
    }

    public class SystemNodeEntityMap : EntityTypeConfiguration<SystemNodeEntity>
    {
        public SystemNodeEntityMap()
        {
            //表定义
            HasKey(t => new { t.PartitioningKey, t.NodeID });
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
