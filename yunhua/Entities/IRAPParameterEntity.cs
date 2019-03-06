using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IRAPBase.Entities
{
    public class IRAPParameterEntity : BaseEntity
    {
        public long PartitioningKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ParameterID { get; set; }
        public int ParameterNameID { get; set; }
        public int ParameterValue { get; set; }
        public string ParameterValueStr { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime TimeUpdated { get; set; }
    }

    public class IRAPParameterMap : 
        EntityTypeConfiguration<IRAPParameterEntity>
    {
        public IRAPParameterMap()
        {
            ToTable("stb000");
            HasKey(t => new { t.PartitioningKey, t.ParameterID });

            Property(t => t.ParameterNameID).IsRequired();
            Property(t => t.ParameterValue).IsRequired();
            Property(t => t.UpdatedBy).IsRequired();
            Property(t => t.TimeUpdated).IsRequired();
        }
    }
}
