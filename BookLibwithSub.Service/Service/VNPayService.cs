using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;
using Microsoft.Extensions.Options;

namespace BookLibwithSub.Service.Service
{
    public class VNPayService : IVNPayService
    {
        private readonly VNPayOptions _options;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPaymentService _paymentService;

        public VNPayService(
            IOptions<VNPayOptions> options,
            ITransactionRepository transactionRepository,
            IPaymentService paymentService)
        {
            _options = options.Value;
            _transactionRepository = transactionRepository;
            _paymentService = paymentService;
        }

        public async Task<VNPayCreateOrderResult> CreateOrderAsync(int transactionId, int userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null || transaction.UserID != userId)
                throw new InvalidOperationException("Transaction not found");

            var orderId = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{transactionId}";
            var amount = (long)(transaction.Amount * 100); // VNPay expects amount in VND cents
            var orderInfo = $"Payment for transaction #{transactionId}";
            var returnUrl = _options.ReturnUrl;
            var callbackUrl = _options.CallbackUrl;

            var vnpay = new SortedList<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", _options.Command },
                { "vnp_TmnCode", _options.TmnCode },
                { "vnp_Amount", amount.ToString() },
                { "vnp_CurrCode", _options.CurrCode },
                { "vnp_BankCode", "" }, // Let user choose bank
                { "vnp_TxnRef", orderId },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", _options.Locale },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_IpAddr", "127.0.0.1" }, // In production, get actual IP
                { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") }
            };

            // Add callback URL if provided
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                vnpay.Add("vnp_Url", callbackUrl);
            }

            var paymentUrl = CreateRequestUrl(vnpay);

            return new VNPayCreateOrderResult
            {
                Success = true,
                PaymentUrl = paymentUrl,
                Message = "Payment URL created successfully",
                OrderId = orderId
            };
        }

        public async Task<VNPayCallbackResult> HandleCallbackAsync(VNPayCallbackRequest request)
        {
            // Validate the callback
            if (!ValidateCallback(request))
            {
                return new VNPayCallbackResult 
                { 
                    Success = false, 
                    Message = "Invalid callback signature" 
                };
            }

            // Check if payment was successful
            if (request.ResponseCode != "00")
            {
                return new VNPayCallbackResult 
                { 
                    Success = false, 
                    Message = $"Payment failed with code: {request.ResponseCode}" 
                };
            }

            // Parse transaction ID from TxnRef
            if (!TryParseTransactionId(request.TxnRef, out var transactionId))
            {
                return new VNPayCallbackResult 
                { 
                    Success = false, 
                    Message = "Invalid transaction reference" 
                };
            }

            // Mark transaction as paid
            await _paymentService.MarkTransactionPaidAsync(transactionId);

            return new VNPayCallbackResult 
            { 
                Success = true, 
                TransactionId = transactionId, 
                Message = "Payment processed successfully",
                OrderId = request.TxnRef
            };
        }

        private string CreateRequestUrl(SortedList<string, string> requestData)
        {
            var data = new StringBuilder();
            foreach (var kvp in requestData.Where(kvp => !string.IsNullOrEmpty(kvp.Value)))
            {
                data.Append($"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}&");
            }

            var querystring = data.ToString().TrimEnd('&');
            var hashData = querystring;
            var secureHash = HmacSHA512(_options.HashSecret, hashData);

            return $"{_options.BaseUrl}?{querystring}&vnp_SecureHash={secureHash}";
        }

        private bool ValidateCallback(VNPayCallbackRequest request)
        {
            var callbackData = new SortedList<string, string>();
            var properties = typeof(VNPayCallbackRequest).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(request)?.ToString();
                if (!string.IsNullOrEmpty(value) && prop.Name != "SecureHash")
                {
                    callbackData.Add(prop.Name, value);
                }
            }

            var data = new StringBuilder();
            foreach (var kvp in callbackData.Where(kvp => !string.IsNullOrEmpty(kvp.Value)))
            {
                data.Append($"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}&");
            }

            var hashData = data.ToString().TrimEnd('&');
            var secureHash = HmacSHA512(_options.HashSecret, hashData);

            return secureHash.Equals(request.SecureHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static bool TryParseTransactionId(string txnRef, out int transactionId)
        {
            transactionId = 0;
            var idx = txnRef.IndexOf('_');
            if (idx <= 0 || idx >= txnRef.Length - 1) return false;
            var idStr = txnRef[(idx + 1)..];
            return int.TryParse(idStr, out transactionId);
        }
    }
}
