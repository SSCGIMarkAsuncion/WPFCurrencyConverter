using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPFCurrencyConverter.Utils
{
    class ApiResult
    {
        public Dictionary<string, double> rates { get; set; }
    }

    public class ServiceConverter
    {

        public static readonly string[] CURRENCIES = {
            "USD", // US Dollar
            "EUR", // Euro
            "JPY", // Japanese Yen
            "GBP", // British Pound
            "AUD", // Australian Dollar
            "CAD", // Canadian Dollar
            "CHF", // Swiss Franc
            "CNY", // Chinese Yuan
            "HKD", // Hong Kong Dollar
            "NZD", // New Zealand Dollar
            "SEK", // Swedish Krona
            "KRW", // South Korean Won
            "SGD", // Singapore Dollar
            "NOK", // Norwegian Krone
            "MXN", // Mexican Peso
            "INR", // Indian Rupee
            "RUB", // Russian Ruble
            "ZAR", // South African Rand
            "BRL", // Brazilian Real,
            "PHP", // Philippine Peso
            "MYR", // Malaysian Ringgit
            "IDR", // Indonesian Rupiah
            "THB", // Thai Baht
            "VND", // Vietnamese Dong
            "KHR", // Cambodian Riel
            "MMK", // Myanmar Kyat
            "LAK", // Lao Kip
            "BND", // Brunei Dollar
            "MOP", // Macanese Pataca (near SEA, used in Macau)
            "TWD"  // New Taiwan Dollar (East Asia, but often grouped regionally)
        };
        private double FromAmount;
        private string FromType;
        private string ToType;

        public ServiceConverter(double amount, string fromType, string toType)
        {
            FromAmount = amount;
            FromType = fromType;
            ToType = toType;
        }

        public async Task<double> Convert()
        {
            if (FromType == ToType) return FromAmount;
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"https://open.er-api.com/v6/latest/{FromType}");
            try
            {
                ApiResult? res = await client.GetFromJsonAsync<ApiResult>(uri);
                if (res == null) throw new Exception("Res is null");
                double rate = res.rates[ToType];
                return FromAmount* rate;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType()}::{e.Message}");
                throw new Exception("Failed to Convert Amount");
            }
        }
    }
}
