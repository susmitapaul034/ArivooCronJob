using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TransactionSchedularAdjustment
    {
        public int TransactionMasterID { get; set; }
        //public string ERP_Remarks { get; set; }
        //public string? ERP_Error { get; set; }
        public bool isCompleted { get; set; }
        public bool CronCanBeProcess { get; set; }
        public bool isfromCron { get; set; }
        public bool isfailed { get; set; }
        public string ERP_Response { get; set; }
}
}
