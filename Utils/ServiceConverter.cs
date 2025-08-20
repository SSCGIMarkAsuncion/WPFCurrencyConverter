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
            "USD",
            "EUR",
            "JPY",
            "GBP",
            "AUD",
            "CAD",
            "CHF",
            "CNY",
            "HKD",
            "NZD",
            "SEK",
            "KRW",
            "SGD",
            "NOK",
            "MXN",
            "INR",
            "RUB",
            "ZAR",
            "BRL"
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
