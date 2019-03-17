using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public  class BaseGenAttrEntity:BaseEntity
    {
       
         public long PartitioningKey { get; set; }

         public int EntityID { get; set; }
        //public string Name { get; set; }
        //public string Value { get; set; }

        //public string Type { get; set; }   
    }
}
