using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    public class StationEntity : BaseEntity
    {
        public string StationID { get; set; }
        public int CommunityID { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public int AgencyID { get; set; }
        public int StationGroupID { get; set; }
        public byte Status { get; set; }
        public string Configuration { get; set; }
        public long StationParameter { get; set; }
        public string StationStrParameters { get; set; }
        public long LastSysLogID { get; set; }
    }
}
