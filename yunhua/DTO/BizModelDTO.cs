using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class OpTypeDTO
    {

        public byte OpType { get; set; }
        public string OpTypeCode { get; set; }
        public string OpTypeName { get; set; }
        public int Ordinal { get; set; }
        public string AuxTranTBLName { get; set; }
        public string OLTPTempFactTBLName { get; set; }
        public string OLTPFixedFactTBLName { get; set; }
        public string AuxFactTBLName { get; set; }
        public string ComplementaryRule { get; set; }
        public string StateExclCtrlStr { get; set; }
        public bool EntityCreating { get; set; }
        public bool BusinessDateIsValid { get; set; }
        public string AuthorizingCondition { get; set; }
    }

    public class BizDimDTO  {

        public int Ordinal { get; set; }
        public int DimTreeID { get; set; }

        public string DimName { get; set; }
     }

    public class BizMetDTO
    {
        public int Ordinal { get; set; }
        public string MetricName { get; set; }

        public byte Scale { get; set; }

        public string UnitOfMeasure { get; set; }

    }

    public class BizRSFactDTO
    {
        public int Ordinal { get; set; }
        public string RSFactName { get; set; }
        public string RSFactTBLName { get; set; }
        public string ProcOnRSFactSave { get; set; }
        public string ProcOnRSFactAppend { get; set; }
        public string ProcOnRSFactDelete { get; set; }
    }

 
}
