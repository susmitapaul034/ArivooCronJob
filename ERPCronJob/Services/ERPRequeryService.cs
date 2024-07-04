using ERPCronJob.Interfaces;
using Quartz.Impl.AdoJobStore;
using System.Data.Common;
using System.Net.Mime;
using System.Net;
using System.Text;
using DBAccess.Interface;
using DBAccess.Implementation;
using Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using static ERPCronJob.Enumerator;
using System.Collections.Generic;

namespace ERPCronJob.Services
{
    public class ERPRequeryService : IERPRequeryService
    {
        private readonly ILogger<ERPRequeryService> _logger;
        //private IDBAccess _data;
        private readonly string _authkey;
        private readonly string _baseurl;
        private readonly IRequeryService _requeryService;


        public ERPRequeryService(ILogger<ERPRequeryService> logger,IConfiguration configuration,IRequeryService requeryService)
        {
            _logger = logger;
            //_data = data;
            _authkey = configuration.GetValue<string>("LMSAuthKey");
            _baseurl = configuration.GetValue<string>("LMSBaseURL");
            _requeryService = requeryService;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ERPRequeryService is working.");
            List<ValidTransactionforPaymentGatewayRequest> validTransactionsForERPSattlement = await _requeryService.GetValidTransactionsForERPSattlement();
            PGPaymentResponseModel pGPaymentResponseModel = new PGPaymentResponseModel();
            APIResponse<List<Transaction>> aPIResponse=new APIResponse<List<Transaction>>();
            TransactionSchedularAdjustment transactionSchedularAdjustment = new TransactionSchedularAdjustment();

            foreach (var validTransaction in validTransactionsForERPSattlement)
            {
                transactionSchedularAdjustment.isfromCron = true;
                if (validTransaction.CanbeProcessForERPSattlement == true && validTransaction.CronCanBeProcess == true && (validTransaction.Is_PG_Failure == true || validTransaction.Is_PG_Sucess == true))
                {
                    if (validTransaction.Is_PG_Failure == true)
                    {

                        aPIResponse = await MakeCancleTransactionAPI(validTransaction.OrderID, validTransaction.TransactionNo, validTransaction);

                        transactionSchedularAdjustment.isfailed = true;
                        transactionSchedularAdjustment.TransactionMasterID = validTransaction.I_Transaction_Master_ID;

                        if (aPIResponse != null)
                        {
                            if (aPIResponse.data != null)
                            {
                                //transactionSchedularAdjustment.ERP_Remarks = aPIResponse.message;
                                transactionSchedularAdjustment.isCompleted = true;
                                transactionSchedularAdjustment.CronCanBeProcess = false;
                                transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                            }
                            else
                            {
                                //transactionSchedularAdjustment.ERP_Remarks = aPIResponse.message;
                                //transactionSchedularAdjustment.ERP_Error = aPIResponse.message;
                                transactionSchedularAdjustment.isCompleted = false;
                                transactionSchedularAdjustment.CronCanBeProcess = true;
                                transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                            }

                        }
                        else
                        {
                            //transactionSchedularAdjustment.ERP_Remarks = null;
                            //transactionSchedularAdjustment.ERP_Error = null;
                            transactionSchedularAdjustment.isCompleted = false;
                            transactionSchedularAdjustment.CronCanBeProcess = true;
                            transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                        }



                    }

                    else
                    {
                        if (validTransaction.Is_PG_Sucess == true)
                        {

                            aPIResponse = await MakeAdjustTransactionAPI(validTransaction.OrderID, validTransaction.TransactionNo, validTransaction);

                            transactionSchedularAdjustment.isfailed = false;
                            transactionSchedularAdjustment.TransactionMasterID = validTransaction.I_Transaction_Master_ID;

                            if (aPIResponse != null)
                            {
                                if (aPIResponse.data != null)
                                {
                                    //transactionSchedularAdjustment.ERP_Remarks = aPIResponse.message;
                                    transactionSchedularAdjustment.isCompleted = true;
                                    transactionSchedularAdjustment.CronCanBeProcess = false;
                                    transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                                }
                                else
                                {
                                    //transactionSchedularAdjustment.ERP_Remarks = aPIResponse.message;
                                    //transactionSchedularAdjustment.ERP_Error = aPIResponse.message;
                                    transactionSchedularAdjustment.isCompleted = false;
                                    transactionSchedularAdjustment.CronCanBeProcess = true;
                                    transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                                }

                            }
                            else
                            {
                                //transactionSchedularAdjustment.ERP_Remarks = null;
                                //transactionSchedularAdjustment.ERP_Error = null;
                                transactionSchedularAdjustment.isCompleted = false;
                                transactionSchedularAdjustment.CronCanBeProcess = true;
                                transactionSchedularAdjustment.ERP_Response = JsonConvert.SerializeObject(aPIResponse);
                            }



                        }


                    }






                    await _requeryService.SaveERPResponse(transactionSchedularAdjustment);
                }
            }


            await Task.Delay(1000 * 20, cancellationToken);
        }





        [HttpGet]
        public async Task<APIResponse<List<Transaction>>> MakeCancleTransactionAPI(string OrderID, string TransactionNo, ValidTransactionforPaymentGatewayRequest ERPRequest)
        {
           
           
            InternalPaymentDetails internalPaymentDetails = new InternalPaymentDetails();
            internalPaymentDetails.internalPaymentDto = await _requeryService.CheckArivooPaymentStatus(TransactionNo);

            StudentPayablePaymentDto studentPayablePaymentDto = new StudentPayablePaymentDto();

            internalPaymentDetails.pGPaymentResponseModel = JsonConvert.DeserializeObject<PGPaymentResponseModel>(ERPRequest.PG_Response);


            if (!internalPaymentDetails.pGPaymentResponseModel.IsSuccess)
            {
                
                StudentDuePayment studentDuePayment = new StudentDuePayment();

                if (internalPaymentDetails.internalPaymentDto.PaymentJson != null)
                {
                    studentDuePayment = JsonConvert.DeserializeObject<StudentDuePayment>(internalPaymentDetails.internalPaymentDto.PaymentJson);
                }

                studentPayablePaymentDto = ConvertToStudentPayablePaymentDto(studentDuePayment);
                studentPayablePaymentDto.TransactionStatus = GetDescription(TransactionDBStatus.Failure);


                studentPayablePaymentDto.pGDetailsPayabletDto.RequestUserId = "1";
                studentPayablePaymentDto.pGDetailsPayabletDto.RequestType = "Requery";
                studentPayablePaymentDto.pGDetailsPayabletDto.PaymentStatus = "Cancelled";
                studentPayablePaymentDto.pGDetailsPayabletDto.SourceOfRequestType = "System_Processed";

                if (ERPRequest.Is_Failed_User== null || ERPRequest.Is_Failed_User == false)
                {
                    studentPayablePaymentDto.TransactionMode = internalPaymentDetails.pGPaymentResponseModel.items[0].method;
                    studentPayablePaymentDto.pGDetailsPayabletDto.PaymentStatus = internalPaymentDetails.pGPaymentResponseModel.items[0].status.ToString();
                    studentPayablePaymentDto.pGDetailsPayabletDto.PgResponse = JsonConvert.SerializeObject(internalPaymentDetails.pGPaymentResponseModel);

                    // Assuming internalPaymentDetails.pGPaymentResponseModel.items[0].created_at is the Unix timestamp in seconds
                    long unixTimeStamp = (long)internalPaymentDetails.pGPaymentResponseModel.items[0].created_at;

                    // Unix timestamp is in seconds, so convert it to DateTime
                    DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);

                    // Assign the converted DateTime to your DTO property
                    studentPayablePaymentDto.pGDetailsPayabletDto.ExecutionDate = dto.DateTime;


                    studentPayablePaymentDto.pGDetailsPayabletDto.ExternalReceiptNo = internalPaymentDetails.pGPaymentResponseModel.items[0].id;

                    if (internalPaymentDetails.pGPaymentResponseModel.items[0].error_code != null)
                    {
                        // Assuming internalPaymentDetails.pGPaymentResponseModel.items[0].created_at is the Unix timestamp in seconds
                        unixTimeStamp = (long)internalPaymentDetails.pGPaymentResponseModel.items[0].created_at;

                        // Unix timestamp is in seconds, so convert it to DateTime
                        dto = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);

                        // Assign the converted DateTime to your DTO property
                        studentPayablePaymentDto.pGDetailsPayabletDto.CancelledDate = dto.DateTime;

                        // studentPayablePaymentDto.pGDetailsPayabletDto.CancelledDate = DateTimeOffset.FromUnixTimeMilliseconds((long)internalPaymentDetails.pGPaymentResponseModel.items[0].created_at).DateTime;
                        studentPayablePaymentDto.pGDetailsPayabletDto.CancelledBy = internalPaymentDetails.pGPaymentResponseModel.items[0].error_source.ToString();
                        studentPayablePaymentDto.pGDetailsPayabletDto.PgMessage = internalPaymentDetails.pGPaymentResponseModel.items[0].error_description.ToString();
                    }
                }
                else
                {
                    studentPayablePaymentDto.pGDetailsPayabletDto.PgResponse = null;
                    studentPayablePaymentDto.pGDetailsPayabletDto.ExecutionDate = studentPayablePaymentDto.TransactionDate;
                    studentPayablePaymentDto.pGDetailsPayabletDto.ExternalReceiptNo = null;

                    studentPayablePaymentDto.pGDetailsPayabletDto.CancelledDate = studentPayablePaymentDto.TransactionDate;
                    studentPayablePaymentDto.pGDetailsPayabletDto.CancelledBy = "User";
                    studentPayablePaymentDto.pGDetailsPayabletDto.PgMessage = null;

                }

                try
                {
                    var result = await _requeryService.CancelPendingTransaction(studentPayablePaymentDto);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null; // Handle exception properly, log if needed
                }
            }

            return null; // Handle the case where pGPaymentResponseModel.IsSuccess is true
        }


        [HttpGet]
        public async Task<APIResponse<List<Transaction>>> MakeAdjustTransactionAPI(string OrderID, string TransactionNo, ValidTransactionforPaymentGatewayRequest ERPRequest)
        {
            InternalPaymentDetails internalPaymentDetails = new InternalPaymentDetails();
            internalPaymentDetails.internalPaymentDto = await _requeryService.CheckArivooPaymentStatus(TransactionNo);

            StudentPayablePaymentDto studentPayablePaymentDto = new StudentPayablePaymentDto();

            internalPaymentDetails.pGPaymentResponseModel = JsonConvert.DeserializeObject<PGPaymentResponseModel>(ERPRequest.PG_Response);


            if (internalPaymentDetails.pGPaymentResponseModel.IsSuccess)
            {

                // internalPaymentDetails.internalPaymentDto = await _feesProvider.CheckArivooPaymentStatus(TransactionNo);

                StudentDuePayment studentDuePayment = new StudentDuePayment();

                if (internalPaymentDetails.internalPaymentDto.PaymentJson != null)
                {
                    studentDuePayment = JsonConvert.DeserializeObject<StudentDuePayment>(internalPaymentDetails.internalPaymentDto.PaymentJson);

                }


                studentPayablePaymentDto = ConvertToStudentPayablePaymentDto(studentDuePayment);


                studentPayablePaymentDto.TransactionDate = DateTime.Now;
                studentPayablePaymentDto.TransactionStatus = GetDescription(TransactionDBStatus.Success);
                studentPayablePaymentDto.TransactionMode = internalPaymentDetails.pGPaymentResponseModel.items[0].method;
                studentPayablePaymentDto.TotalTransactionAmount = Convert.ToDecimal((internalPaymentDetails.pGPaymentResponseModel.items[0].amount) / 100);


                studentPayablePaymentDto.pGDetailsPayabletDto.SourceOfRequestType = "System_Processed";
                studentPayablePaymentDto.pGDetailsPayabletDto.RequestUserId = "1";
                studentPayablePaymentDto.pGDetailsPayabletDto.CancelledDate = null;
                studentPayablePaymentDto.pGDetailsPayabletDto.CancelledBy = null;
                studentPayablePaymentDto.pGDetailsPayabletDto.PaymentStatus = GetDescription(TransactionDBStatus.Success);
                studentPayablePaymentDto.pGDetailsPayabletDto.PgResponse = JsonConvert.SerializeObject(internalPaymentDetails.pGPaymentResponseModel);
                studentPayablePaymentDto.pGDetailsPayabletDto.PgMessage = studentPayablePaymentDto.pGDetailsPayabletDto.PaymentStatus;

                studentPayablePaymentDto.pGDetailsPayabletDto.RequestType = "Requery";
                // Assuming internalPaymentDetails.pGPaymentResponseModel.items[0].created_at is the Unix timestamp in seconds
                long unixTimeStamp = (long)internalPaymentDetails.pGPaymentResponseModel.items[0].created_at;

                // Unix timestamp is in seconds, so convert it to DateTime
                DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);

                // Assign the converted DateTime to your DTO property
                studentPayablePaymentDto.pGDetailsPayabletDto.ExecutionDate = dto.DateTime;

                studentPayablePaymentDto.pGDetailsPayabletDto.ExternalReceiptNo = internalPaymentDetails.pGPaymentResponseModel.items[0].id;



                try
                {
                    var result = await _requeryService.AdjustPendingTransaction(studentPayablePaymentDto);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null; // Handle exception properly, log if needed
                }


            }




            return null;

        }


        public static StudentPayablePaymentDto ConvertToStudentPayablePaymentDto(StudentDuePayment source)
        {
            var target = new StudentPayablePaymentDto
            {
                StudentID = source.StudentID,
                CenterID = source.CenterID,
                TransactionNo = source.TransactionNo,
                TransactionDate = source.TransactionDate,
                TransactionStatus = source.TransactionStatus,
                TransactionSource = source.TransactionSource,
                TransactionMode = source.TransactionMode,
                Amount = source.Amount,
                Tax = source.Tax,
                TotalTransactionAmount = source.TotalTransactionAmount,
                paymentGatewayBrandID = source.paymentGatewayBrandID,
                smsPaymentMode = source.smsPaymentMode,
                Order_Id = source.Order_Id,

                pGDetailsPayabletDto = new PGDetailsPayabletDto
                {
                    SourceOfRequestType = source.pGDetailsDto.SourceOfRequestType,
                    RequestUserId = source.pGDetailsDto.RequestUserId,
                    CancelledBy = source.pGDetailsDto.CancelledBy,
                    CancelledDate = source.pGDetailsDto.CancelledDate,
                    ExternalReceiptNo = source.pGDetailsDto.ExternalReceiptNo,
                    PaymentStatus = source.pGDetailsDto.PaymentStatus,
                    PgResponse = source.pGDetailsDto.PgResponse,
                    PgMessage = source.pGDetailsDto.PgMessage,
                    RequestType = source.pGDetailsDto.RequestType,
                    ExecutionDate = source.pGDetailsDto.ExecutionDate
                },

                FeeSchedulePaymentDetails = source.FeeSchedulePaymentDetails.Select(fsp => new FeeSchedulePayableDto
                {
                    FeeScheduleID = fsp.FeeScheduleID,
                    Amount = fsp.Amount,
                    Tax = fsp.Tax,
                    TotalAmount = fsp.TotalAmount,
                    InvoicePaymentDetails = fsp.InvoicePaymentDetails
                        .GroupBy(ip => new { ip.InvoiceNo, ip.InstallmentDate })
                        .Select(g => new InvoicePayableDto
                        {
                            InvoiceNo = g.Key.InvoiceNo,
                            InstallmentDate = g.Key.InstallmentDate,
                            FeeComponentPayableDetails = g.Select(fc => new FeeComponentPayableDto
                            {
                                InvoiceDetailID = fc.InvoiceDetailID,
                                FeeComponentID = fc.FeeComponentID,
                                Amount = fc.Amount,
                                Tax = fc.Tax,
                                TotalAmount = fc.TotalAmount,
                                IsAdhoc = fc.IsAdhoc
                            }).ToList()
                        }).ToList()
                }).ToList()
            };

            return target;
        }


        public static string GetDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }


    }
}
