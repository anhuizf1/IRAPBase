using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Entities
{
    [Table("stb011")]
    public  class SystemEntity : BaseEntity
    {

        public int Ordinal { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//不自动增长
        public int SystemID { get; set; }
        public string VersionNo { get; set; }
        public string ProductNo { get; set; }
        public string GAYearMonth { get; set; }
        public string Author { get; set; }
        public string Coauthor { get; set; }
        public string LogoPicPath { get; set; }
        public byte[] LogoPic { get; set; }
        public string BGPicPath { get; set; }
        public byte[] BackgroundPic { get; set; }
        public Int16 VoiceChannelMin { get; set; }
        public Int16 VoiceChannelMax { get; set; }
        public string TelephoneNo { get; set; }
        public string VoiceFilePath { get; set; }
        public int DefaultDataSrcLinkID { get; set; }
        public string DefaultAppServer { get; set; }
    }
}
