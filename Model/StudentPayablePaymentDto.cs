using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StudentPayablePaymentDto
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
        public List<FeeSchedulePayableDto> FeeSchedulePaymentDetails { get; set; } = new List<FeeSchedulePayableDto>();
        public PGDetailsPayabletDto pGDetailsPayabletDto { get; set; } = new PGDetailsPayabletDto();
        public string Order_Id { get; set; }
    }

    public class PGDetailsPayabletDto
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

    public class FeeSchedulePayableDto
    {
        public int FeeScheduleID { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoicePayableDto> InvoicePaymentDetails { get; set; } = new List<InvoicePayableDto>();
    }
    public class InvoicePayableDto
    {
        public string InvoiceNo { get; set; }
        public DateTime InstallmentDate { get; set; }

        public List<FeeComponentPayableDto> FeeComponentPayableDetails { get; set; } = new List<FeeComponentPayableDto>();
    }

    public class FeeComponentPayableDto
    {
        public int? InvoiceDetailID { get; set; }
        public int FeeComponentID { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsAdhoc { get; set; }
    }
}
