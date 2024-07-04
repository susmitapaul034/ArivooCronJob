using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StudentDuePayment
    {
        public string StudentID { get; set; }
        public int CenterID { get; set; }
        public string TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionStatus { get; set; } = "Initiated";
        public string TransactionSource { get; set; }
        public string? TransactionMode { get; set; } = null;
        public decimal? Amount { get; set; }
        public decimal? Tax { get; set; }
        public decimal TotalTransactionAmount { get; set; }
        public int paymentGatewayBrandID { get; set; }
        public int smsPaymentMode { get; set; }
        public List<FeeScheduleDue> FeeSchedulePaymentDetails { get; set; } = new List<FeeScheduleDue>();
        public string Order_Id { get; set; }
        public PGDetailsPaymentDto pGDetailsDto { get; set; } = new PGDetailsPaymentDto();
    }


    public class PGDetailsPaymentDto
    {
        public string SourceOfRequestType { get; set; } = "System_Processed";
        public string RequestUserId { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? ExternalReceiptNo { get; set; }
        public string PaymentStatus { get; set; } = "Success / Cancelled / Rejected";
        public string? PgResponse { get; set; }
        public string? PgMessage { get; set; }
        public string RequestType { get; set; } = "Requery";
        public DateTime ExecutionDate { get; set; }

    }

    public class FeeScheduleDue
    {
        public int FeeScheduleID { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoiceDue> InvoicePaymentDetails { get; set; } = new List<InvoiceDue>();
    }
    public class InvoiceDue
    {
        public string InvoiceNo { get; set; }
        public int? InvoiceDetailID { get; set; }
        public int FeeComponentID { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsAdhoc { get; set; }
        public DateTime InstallmentDate { get; set; }
    }
}
