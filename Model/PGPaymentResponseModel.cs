using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PGPaymentResponseModel
    {
        public string entity { get; set; }
        public int count { get; set; }
        public List<PGPaymentItem> items { get; set; }
        public string DisplayCustomMessage { get; set; }
        public bool IsSuccess { get; set; } = false;
        public bool IsUserEndCancle { get; set; } = false;
        public bool CanbeProcessForERPSattlement { get; set; } = false;
    }

   

    public class PGPaymentItem
    {
        public string id { get; set; }
        public string entity { get; set; }
        public int? amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public object invoice_id { get; set; }
        public bool? international { get; set; }
        public string method { get; set; }
        public int? amount_refunded { get; set; }
        public object refund_status { get; set; }
        public bool? captured { get; set; }
        public object description { get; set; }
        public object card_id { get; set; }
        public string bank { get; set; }
        public object wallet { get; set; }
        public object vpa { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public List<object> notes { get; set; }
        public int? fee { get; set; }
        public int? tax { get; set; }
        public object error_code { get; set; }
        public object error_description { get; set; }
        public object error_source { get; set; }
        public object error_step { get; set; }
        public object error_reason { get; set; }
        public AcquirerData acquirer_data { get; set; }
        public int? created_at { get; set; }
    }

    public class AcquirerData
    {
        public string bank_transaction_id { get; set; }
    }

    

}
