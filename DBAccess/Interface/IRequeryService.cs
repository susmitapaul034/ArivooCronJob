using Model;
using Models;
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
    }
}
