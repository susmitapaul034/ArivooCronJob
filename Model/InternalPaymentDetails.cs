using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class InternalPaymentDetails
    {
        public InternalPaymentDto internalPaymentDto { get; set; } =new InternalPaymentDto();
        public PGPaymentResponseModel pGPaymentResponseModel { get; set; } =new PGPaymentResponseModel();
    }

    public class InternalPaymentDto
    {
        public string TransactionNo { get; set; }
        public string MobileNo { get; set; }
        public string PaymentJson { get; set; }
        public int PaymentStatusID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int TransactionHistoryID { get; set; }
        public string StatusDescription { get; set; }
        public string StatusColour { get; set; }
        public int PG_History_ID { get; set; }
        public string SourceofRequestType { get; set; }
        public string RequestUserId { get; set; }
        public string? PGCancelledBy { get; set; }
        public DateTime? PGCancelledDate { get; set; }
        public string? ExternalReceiptNo { get; set; }
        public string PaymentStatus { get; set; }
        public string? PGMessage { get; set; }
        public string? PGResponseType { get; set; }
        public DateTime? PGExecutionDate { get; set; }
        public string? PGResponseJson { get; set; }
        public string S_Transaction_No { get; set; }
        public int I_Transaction_Master_ID { get; set; }
        public int InvalidInvoiceCount { get; set; }
        public int ExistingReceiptCount { get; set; }
        public bool IsAllowed { get; set; }
        public int PaymentBrandGateWayID { get; set; }
        public bool IsCompleted { get; set; }
        public bool CanBeProcessed { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime Dt_CreatedAt { get; set; }
        public DateTime UpdateOn { get; set; }


    }

}
