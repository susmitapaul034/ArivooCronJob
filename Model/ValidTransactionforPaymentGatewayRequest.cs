using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ValidTransactionforPaymentGatewayRequest
    {
        public int I_Requery_Cron_Job_ID { get; set; }
        public string S_Transaction_No { get; set; }
        public int I_Transaction_Master_ID { get; set; }
        public string S_Current_Status { get; set; }
        public string CompleteStatus { get; set; }
        public bool? CronCanBeProcess { get; set; }
        public int? PG_NoOfAttempt { get; set; }
        public int? ERP_NoOfAttempt { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool? Is_PG_Sucess { get; set; }
        public bool? Is_PG_Failure { get; set; }
        public bool? Is_Failed_User { get; set; }
        public int? Requery_PG_LogID { get; set; }
        public int? Requery_Request_LogID { get; set; }
        public string PG_Response { get; set; }
        public bool? CanbeProcessForERPSattlement { get; set; }
        public DateTime? Dt_PG_Current_Response_Date { get; set; }
        public DateTime? Dt_ERP_Current_Response_Date { get; set; }
        public DateTime? Dt_Closed_Date_By_Other_Source { get; set; }
        public string S_PG_Remarks { get; set; }
        public string S_ERP_Remarks { get; set; }
        public string S_PG_Error { get; set; }
        public string S_ERP_Error { get; set; }
        public string TransactionNo { get; set; }
        public int BrandID { get; set; }
        public int PaymentGatewayBrandID { get; set; }
        public string Salt {  get; set; }
        public string keySecret { get; set; }
        public string OrderID { get; set; }
    }
}
