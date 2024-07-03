using System.Data;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Models;
using Newtonsoft.Json;
using DBAccess.Interface;
using System.Data.SqlClient;
using Model;
using Dapper;

namespace DBAccess.Implementation
{
    public class RequeryService : IRequeryService
    {
        private IConfiguration Configuration;
        string connectionString;

        public RequeryService(IConfiguration _configuration)
        {
            Configuration = _configuration;
            connectionString = this.Configuration.GetConnectionString("DevConn");
        }


        public async Task<PGPaymentResponseModel> CheckOrderPayment(string orderId, string keyId, string keySecret)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    PGPaymentResponseModel pGPaymentResponseModel = new PGPaymentResponseModel();
                    var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{keyId}:{keySecret}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                    var response = await client.GetAsync($"https://api.razorpay.com/v1/orders/{orderId}/payments");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    //var paymentsDetails = JArray.Parse(responseData);

                    

                    pGPaymentResponseModel = JsonConvert.DeserializeObject<PGPaymentResponseModel>(responseData);

                    if (pGPaymentResponseModel.items != null && pGPaymentResponseModel.items.Count > 0)
                    {

                        foreach (var payment in pGPaymentResponseModel.items)
                        {

                            switch (payment.status.ToLower())
                            {
                                case "captured":
                                    pGPaymentResponseModel.DisplayCustomMessage = "Payment is successfully captured";
                                    pGPaymentResponseModel.IsSuccess = true;
                                    pGPaymentResponseModel.CanbeProcessForERPSattlement= true;
                                    break;
                                case "authorized":
                                    pGPaymentResponseModel.DisplayCustomMessage = "Payment is authorized but not captured";
                                    pGPaymentResponseModel.IsSuccess = true;
                                    pGPaymentResponseModel.CanbeProcessForERPSattlement = false;
                                    break;
                                case "failed":
                                    pGPaymentResponseModel.DisplayCustomMessage = "Payment failed";
                                    pGPaymentResponseModel.IsSuccess = false;
                                    pGPaymentResponseModel.CanbeProcessForERPSattlement = true;
                                    break;
                                case "refunded":
                                    pGPaymentResponseModel.DisplayCustomMessage = "Payment has been refunded";
                                    pGPaymentResponseModel.IsSuccess = false;
                                    pGPaymentResponseModel.CanbeProcessForERPSattlement = false;
                                    break;
                                default:
                                    pGPaymentResponseModel.DisplayCustomMessage = "Unknown payment status";
                                    pGPaymentResponseModel.IsSuccess = false;
                                    pGPaymentResponseModel.CanbeProcessForERPSattlement = false;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        pGPaymentResponseModel.DisplayCustomMessage = "Unknown payment status/ Payment not Initiated With Generated Order ID";
                        pGPaymentResponseModel.IsSuccess = false;
                        pGPaymentResponseModel.CanbeProcessForERPSattlement = true;
                        pGPaymentResponseModel.IsUserEndCancle = true;
                    }

                    return pGPaymentResponseModel;
                }
                catch (HttpRequestException e)
                {
                    return null;
                }
            }
        }

        
        public async Task<string> GetAccessToken(string studentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("dbo.usp_Get_Token_StudentWise", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    await connection.OpenAsync();

                    var token = await command.ExecuteScalarAsync() as string;

                    return token;
                }
            }
        }


        public async Task<List<ValidTransactionforPaymentGatewayRequest>> GetValidTransactionsForPaymentGateawayRecheck()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();

               
                return (List<ValidTransactionforPaymentGatewayRequest>)await connection.QueryAsync<ValidTransactionforPaymentGatewayRequest>("[dbo].[usp_ERP_GET_Valid_Pending_Transaction_For_PaymentGateway]", dynamicParameters, commandType: System.Data.CommandType.StoredProcedure);

            }
        }

        public async Task<int> SavePaymentGatewayResponse(PGPaymentResponseModel pGPaymentResponseModel, int TransactionMasterID, string TransactionNo)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var dynamicParameters = new DynamicParameters();

                    dynamicParameters.Add("S_Transaction_No", TransactionNo);
                    dynamicParameters.Add("I_Transaction_Master_ID", TransactionMasterID);
                    //dynamicParameters.Add("currentStatus", (object)DBNull.Value);
                    //dynamicParameters.Add("CompleteStatus", (object)DBNull.Value);
                    //dynamicParameters.Add("CronCanBeProcess", (object)DBNull.Value);
                    //dynamicParameters.Add("NoOfAttempt", (object)DBNull.Value);
                    //dynamicParameters.Add("StatusID", (object)DBNull.Value);
                    dynamicParameters.Add("Is_PG_Success", pGPaymentResponseModel.IsSuccess);
                    dynamicParameters.Add("Is_PG_Failure", !pGPaymentResponseModel.IsSuccess);
                    dynamicParameters.Add("Is_Failed_User", pGPaymentResponseModel.IsUserEndCancle);
                    //dynamicParameters.Add("Requery_PG_LogID", (object)DBNull.Value);
                    //dynamicParameters.Add("Requery_Request_LogID", (object)DBNull.Value);
                    dynamicParameters.Add("PG_Response", JsonConvert.SerializeObject(pGPaymentResponseModel));
                    //dynamicParameters.Add("ERP_Response", (object)DBNull.Value);
                    dynamicParameters.Add("PG_Remarks", pGPaymentResponseModel.DisplayCustomMessage);
                    //dynamicParameters.Add("ERP_Remarks",(object)DBNull.Value);
                    if (pGPaymentResponseModel == null)
                    {
                        dynamicParameters.Add("PG_Error", "No Response received");
                    }
                    else
                    {
                        if (pGPaymentResponseModel.count <= 0)
                        {
                            dynamicParameters.Add("PG_Error", pGPaymentResponseModel.DisplayCustomMessage);
                        }
                        else
                        {
                            dynamicParameters.Add("PG_Error", (object)DBNull.Value);
                        }
                    }
                    //dynamicParameters.Add("ERP_Error", (object)DBNull.Value);
                    dynamicParameters.Add("CanbeProcessForERPSattlement", pGPaymentResponseModel.CanbeProcessForERPSattlement);
                    dynamicParameters.Add("IsFromCron", true);

                    return await connection.QueryFirstOrDefaultAsync<int>("[dbo].[usp_ERP_SaveTransactionCronJob]", dynamicParameters, commandType: CommandType.StoredProcedure);



                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further analysis if needed
                return -1;
            }
        }


        public async Task<List<ValidTransactionforPaymentGatewayRequest>> GetValidTransactionsForERPSattlement()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();


                return (List<ValidTransactionforPaymentGatewayRequest>)await connection.QueryAsync<ValidTransactionforPaymentGatewayRequest>("[dbo].[usp_ERP_GET_ValidTransactions_For_ERP_Process]", dynamicParameters, commandType: System.Data.CommandType.StoredProcedure);

            }
        }



    }
}
