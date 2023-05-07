using InvoiceGenerator.Helpers;
using InvoiceGenerator.Models.Data;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace InvoiceGenerator.Models.Configuration
{
    public class InvoiceConfiguration
    {
        public string? LogoUrl { get; set; }
        public int DaysToPay { get; set; }
        public bool IncludeTax { get; set; }
        public bool Swedish { get; set; }
        public Company? Sender { get; set; }
        public Company? Receiver { get; set; }
        public PaymentInformation? PaymentInformation { get; set; }
        public int DefaultUnitPrice { get; set; }
        public Dictionary<string, int>? UnitPriceOverride { get; set; }

        public static InvoiceConfiguration Default { get; } = new InvoiceConfiguration
        {
            LogoUrl = "https://avatars.githubusercontent.com/u/71601343",
            DaysToPay = 30,
            IncludeTax = true,
            Swedish = true,
            Sender = new Company
            {
                Name = "Sakur",
                Address = new Address
                {
                    FirstLine = "Gatuadress 1",
                    SecondLine = "123 45 Stad"
                },
                OrganizationNumber = "123456-7890",
                Reference = new Person
                {
                    Name = "Förnamn Efternamn",
                    Email = "email@domain.se",
                    PhoneNumber = "+46 70 123 45 67"
                }
            },
            Receiver = new Company
            {
                Name = "Företag",
                Address = new Address
                {
                    FirstLine = "Gatuadress 1",
                    SecondLine = "123 45 Stad"
                },
                OrganizationNumber = "123456-7890",
                Reference = new Person
                {
                    Name = "Förnamn Efternamn",
                    Email = "email@domain.se"
                }
            },
            PaymentInformation = new PaymentInformation
            {
                BankgiroNumber = "1232-4567",
                Bic = "SWEDSESS",
                Iban = "SE3550000000054910000003"
            },
            DefaultUnitPrice = 750,
            UnitPriceOverride = new Dictionary<string, int>
            {
                { "Benny Karlsson", 1000 },
                { "Henrik Stig", 500 }
            }
        };

        public static InvoiceConfiguration? FromJson(string json)
        {
            return JsonSerializer.Deserialize<InvoiceConfiguration>(json);
        }

        public string ToJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(this, options: options);
        }

        public byte[] GetImageBytes()
        {
            if (LogoUrl == null)
                return new byte[0];

            return ApiHelper.Instance.GetImageBytes(LogoUrl).Result;
        }

        public int GetUnitPrice(string name)
        {
            if (UnitPriceOverride == null)
                return DefaultUnitPrice;

            if (UnitPriceOverride.TryGetValue(name, out int price))
                return price;

            return DefaultUnitPrice;
        }
    }
}
