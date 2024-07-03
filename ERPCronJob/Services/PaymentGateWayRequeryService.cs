using ERPCronJob.Interfaces;
using Quartz.Impl.AdoJobStore;
using System.Data.Common;
using System.Net.Mime;
using System.Net;
using System.Text;
using DBAccess.Interface;
using DBAccess.Implementation;
using Model;
using Models;

namespace ERPCronJob.Services
{
    public class PaymentGateWayRequeryService : IPaymentGateWayRequeryService
    {
        private readonly ILogger<PaymentGateWayRequeryService> _logger;
        //private IDBAccess _data;
        private readonly string _authkey;
        private readonly string _baseurl;
        private readonly IRequeryService _requeryService;


        public PaymentGateWayRequeryService(ILogger<PaymentGateWayRequeryService> logger,IConfiguration configuration,IRequeryService requeryService)
        {
            _logger = logger;
            //_data = data;
            _authkey = configuration.GetValue<string>("LMSAuthKey");
            _baseurl = configuration.GetValue<string>("LMSBaseURL");
            _requeryService = requeryService;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} PaymentGateWayRequeryService is working.");
            List<ValidTransactionforPaymentGatewayRequest> validTransactionforPaymentGatewayRequests = await _requeryService.GetValidTransactionsForPaymentGateawayRecheck();
            PGPaymentResponseModel pGPaymentResponseModel = new PGPaymentResponseModel();
            int flag = 0;

            foreach (var validtransaction in validTransactionforPaymentGatewayRequests)
            {
                pGPaymentResponseModel =await  _requeryService.CheckOrderPayment(validtransaction.OrderID, validtransaction.Salt, validtransaction.keySecret);
                flag=await _requeryService.SavePaymentGatewayResponse(pGPaymentResponseModel, validtransaction.I_Transaction_Master_ID, validtransaction.S_Transaction_No);

            }

            await Task.Delay(1000 * 20, cancellationToken);
        }

    }
}
