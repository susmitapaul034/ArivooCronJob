using System.ComponentModel;

namespace ERPCronJob
{
    public class Enumerator
    {
        public enum TransactionDBStatus
        {
            [Description("Initiated")]
            Initiated,
            [Description("Success")]
            Success,
            [Description("Failure")]
            Failure,
        }
    }
}
