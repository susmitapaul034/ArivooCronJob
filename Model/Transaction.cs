using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Transaction
    {
        public int? TransactionID { get; set; }
        public int BrandID { get; set; }
        public int? CenterID { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionSource { get; set; }
        public DateTime Transactiondate { get; set; }
        public bool CanBeProcessed { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public string? TransactionMode { get; set; }
        public decimal TotalTransactionAmount { get; set; }
        public int PaymentGatewayBrandID { get; set; }
        public int SmsPaymentMode { get; set; }
        public string StudentID { get; set; }
        public string XmlData { get; set; }
        public string Paymentjson { get; set; }
        public int StatusID { get; set; }
        public string MobileNo { get; set; }

        public List<TransactionInvoiceDetails> TransactionInvoiceDetails { get; set; } = new List<TransactionInvoiceDetails>();
        public string Order_Id { get; set; }
        public TransactionPGDetailsPaymentDto TransactionPGDetailsDto { get; set; } = new TransactionPGDetailsPaymentDto();
    }

    public class TransactionInvoiceDetails
    {
        public int TransactionID { get; set; }
        public int InvoiceHeaderID { get; set; }
        public string InstallmentInvoiceNo { get; set; }
        public bool IsAdhoc { get; set; }
        public string StatusValue { get; set; }
        public string OnAccountComponent { get; set; }
        public DateTime InstallmentDate { get; set; }
        public decimal InstallmentBaseAmountPaid { get; set; }
        public decimal InstallmentTaxAmountPaid { get; set; }
        public bool CanBeProcessed { get; set; }
        public bool IsCompleted { get; set; }
        public int? ReceiptHeaderID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
    }


    public class TransactionPGDetailsPaymentDto
    {
        public string SourceOfRequestType { get; set; } = "Web/App";
        public string RequestUserId { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? ExternalReceiptNo { get; set; }
        public string PaymentStatus { get; set; } = "Success / Cancelled / Rejected";
        public string? PgResponse { get; set; }
        public string? PgMessage { get; set; }
        public string RequestType { get; set; } = "Initiation / PostPayment / Requery";
        public DateTime ExecutionDate { get; set; }

    }

}
