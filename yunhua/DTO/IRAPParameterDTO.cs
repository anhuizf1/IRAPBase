using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class IRAPParameterDTO
    {
        public long PartitioningKey { get; set; }
        public byte ParameterID { get; set; }
        public int ParameterNameID { get; set; }
        public string ParameterName { get; set; }
        public int ParameterValue { get; set; }
        public string ParameterValueStr { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}
