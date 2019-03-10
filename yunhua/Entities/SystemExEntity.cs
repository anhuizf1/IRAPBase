using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class SystemExEntity:BaseEntity
    {
        public int SystemID { get; set; }
        public Int16 ProgLanguageID { get; set; }
        public byte MenuStyle1 { get; set; }
        public byte MenuStyle2 { get; set; }
        public byte MenuAnchor1 { get; set; }
        public byte MenuAnchor2 { get; set; }
        public byte MenuShowCtrl { get; set; }
        public bool AddToolBar { get; set; }
        public byte ScreenResolution { get; set; }
        public string KeyStrokeStream { get; set; }
    }


    public class SystemExEntityMap : EntityTypeConfiguration<SystemExEntity>
    {
        public SystemExEntityMap()
        {
            //表定义
            ToTable("stb011_Ex1");
            HasKey(t => new { t.SystemID, t.ProgLanguageID });

            Property(t => t.MenuStyle1).IsRequired();
            Property(t => t.MenuStyle2).IsRequired();
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
