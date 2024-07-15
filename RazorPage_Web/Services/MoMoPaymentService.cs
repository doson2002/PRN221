using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RazorPage_Web.Services
{
	public class MoMoPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;

		public MoMoPaymentService(IConfiguration configuration, HttpClient httpClient)
		{
			_configuration = configuration;
			_httpClient = httpClient;
		}

		public async Task<string> CreatePaymentRequest(string orderId, long amount, string orderInfo)
		{
			string endpoint = _configuration["MoMo:Endpoint"];
			string partnerCode = _configuration["MoMo:PartnerCode"];
			string accessKey = _configuration["MoMo:AccessKey"];
			string secretKey = _configuration["MoMo:SecretKey"];
            string returnUrl = _configuration["MoMo:ReturnUrl"];// URL bạn muốn MoMo redirect về sau khi thanh toán
            string notifyUrl = _configuration["MoMo:NotifyUrl"];// URL để nhận thông báo từ MoMo

            string requestId = Guid.NewGuid().ToString();
			string requestType = "captureWallet";
			string extraData = "";

            // In ra các giá trị để kiểm tra
            Console.WriteLine($"partnerCode: {partnerCode}");
            Console.WriteLine($"accessKey: {accessKey}");
            Console.WriteLine($"secretKey: {secretKey}");
            Console.WriteLine($"returnUrl: {returnUrl}");
            Console.WriteLine($"notifyUrl: {notifyUrl}");
            Console.WriteLine($"requestId: {requestId}");
            Console.WriteLine($"orderId: {orderId}");
            Console.WriteLine($"amount: {amount}");
            Console.WriteLine($"orderInfo: {orderInfo}");

            string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";
			string signature = SignSHA256(rawHash, secretKey);


            // In ra rawHash và signature để kiểm tra
            Console.WriteLine($"rawHash: {rawHash}");
            Console.WriteLine($"signature: {signature}");

            var message = new
			{
				partnerCode,
				partnerName = "Test",
				storeId = "TestStore",
				requestId,
				amount,
				orderId,
				orderInfo,
				redirectUrl = returnUrl,
				ipnUrl = notifyUrl,
				requestType,
				extraData,
				lang = "vi",
				signature
			};

			var response = await _httpClient.PostAsync(endpoint, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
			var responseContent = await response.Content.ReadAsStringAsync();

			// Log the response content
			Console.WriteLine("Response Content: " + responseContent);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException("Bad Request: " + responseContent);
            }
            var jObject = JObject.Parse(responseContent);
            if (jObject != null && jObject["payUrl"] != null)
            {
                return jObject["payUrl"].ToString();
            }
            else
            {
                // Xử lý trường hợp jObject hoặc jObject["payUrl"] là null
                throw new InvalidOperationException("Không thể tạo yêu cầu thanh toán: jObject hoặc jObject['payUrl'] là null.");
            }
			
        }

		private static string SignSHA256(string data, string key)
		{
			var hash = new HMACSHA256(Encoding.UTF8.GetBytes(key));
			return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
		}
	}
}
