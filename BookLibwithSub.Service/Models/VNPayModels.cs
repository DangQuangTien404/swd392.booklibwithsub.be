using System.Text.Json.Serialization;

namespace BookLibwithSub.Service.Models
{
    public class VNPayOptions
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Locale { get; set; } = "vn";
    }

    public class VNPayCreateOrderResult
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
    }

    public class VNPayCallbackRequest
    {
        [JsonPropertyName("vnp_TmnCode")]
        public string TmnCode { get; set; } = string.Empty;

        [JsonPropertyName("vnp_Amount")]
        public string Amount { get; set; } = string.Empty;

        [JsonPropertyName("vnp_BankCode")]
        public string BankCode { get; set; } = string.Empty;

        [JsonPropertyName("vnp_BankTranNo")]
        public string BankTranNo { get; set; } = string.Empty;

        [JsonPropertyName("vnp_CardType")]
        public string CardType { get; set; } = string.Empty;

        [JsonPropertyName("vnp_OrderInfo")]
        public string OrderInfo { get; set; } = string.Empty;

        [JsonPropertyName("vnp_PayDate")]
        public string PayDate { get; set; } = string.Empty;

        [JsonPropertyName("vnp_ResponseCode")]
        public string ResponseCode { get; set; } = string.Empty;

        [JsonPropertyName("vnp_TxnRef")]
        public string TxnRef { get; set; } = string.Empty;

        [JsonPropertyName("vnp_SecureHash")]
        public string SecureHash { get; set; } = string.Empty;

        [JsonPropertyName("vnp_TransactionNo")]
        public string TransactionNo { get; set; } = string.Empty;

        [JsonPropertyName("vnp_TransactionStatus")]
        public string TransactionStatus { get; set; } = string.Empty;
    }

    public class VNPayCallbackResult
    {
        public bool Success { get; set; }
        public int TransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
    }
}
