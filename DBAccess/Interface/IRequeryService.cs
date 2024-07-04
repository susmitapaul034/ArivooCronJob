using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Interface
{
    public interface IRequeryService
    {
        Task<PGPaymentResponseModel> CheckOrderPayment(string orderId, string keyId, string keySecret);
        Task<List<ValidTransactionforPaymentGatewayRequest>> GetValidTransactionsForPaymentGateawayRecheck();
        Task<int> SavePaymentGatewayResponse(PGPaymentResponseModel pGPaymentResponseModel, int TransactionMasterID, string TransactionNo);
        Task<List<ValidTransactionforPaymentGatewayRequest>> GetValidTransactionsForERPSattlement();
        Task<InternalPaymentDto> CheckArivooPaymentStatus(string TransactionNo);

        Task<APIResponse<List<Transaction>>> CancelPendingTransaction(StudentPayablePaymentDto studentPayablePaymentDto);
        Task<APIResponse<List<Transaction>>> AdjustPendingTransaction(StudentPayablePaymentDto studentPayablePaymentDto);
        Task<int> SaveERPResponse(TransactionSchedularAdjustment transactionSchedularAdjustment);
    }
}
