using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPMES.Entities
{
    [Table("stb101")]
    public class Estb101 : BaseEntity
    {
        [Key]
        public Int16 TreeCorrID { get; set; }
        public string CorrAttrTBLName { get; set; }
        public Int16 TreeID1 { get; set; }
        public Int16 TreeID2 { get; set; }
        public Int16 TreeID3 { get; set; }
        public Int16 TreeID4 { get; set; }
        public Int16 TreeID5 { get; set; }
        public Int16 TreeID6 { get; set; }
        public Int16 TreeID7 { get; set; }
        public Int16 TreeID8 { get; set; }
    }

    public class Estb101Map : EntityTypeConfiguration<Estb101>
    {
        public Estb101Map()
        {
            //表定义
            // ToTable("stb101");
            HasKey(t => t.TreeCorrID);
            //   Property(p => p.LanguageID).HasColumnType("smallint");
        }
    }
}
