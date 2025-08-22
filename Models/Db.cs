using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCurrencyConverter.Models
{
    public class CurrencyContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public CurrencyContext() : base(@"Server=localhost\SQLEXPRESS;Database=Currency;Trusted_Connection=True;")
        { }

        public List<string> GetDistinctCodes()
        {
            List<string> currencies = this.Currencies
                .Select(x => x.Code)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return currencies;
        }
    }

    public class Currency
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public double Rate { get; set; }
        public string ToCode { get; set; }

        public Currency() { }
        public Currency(string code, double rate, string toCode)
        {
            if (string.IsNullOrEmpty(code)) throw new Exception("From cannot be empty");
            if (string.IsNullOrEmpty(toCode)) throw new Exception("To cannot be empty");
            if (toCode == code) throw new Exception("From and To cannot be the same");
            if (rate <= 0.0)  throw new Exception("Rate cannot be less than or equal to zero");

            Id = Guid.NewGuid().ToString();
            Code = code.ToUpper();
            Rate = rate;
            ToCode = toCode.ToUpper();
        }
    }
}
