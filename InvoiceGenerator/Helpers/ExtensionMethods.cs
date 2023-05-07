using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetUnescapedValue(this string value)
        {
            return value.Substring(1, value.Length - 2);
        }

        public static string ToDateString(this DateTime date)
        {
            return date.ToString("dd-MM-yyyy");
        }

        public static string ToPriceString(this int price, string? currency = null)
        {
            string reference = price.ToString();

            StringBuilder result = new StringBuilder();

            int stepDistance = 0;

            for (int i = reference.Length - 1; i >= 0; i--)
            {
                result.Insert(0, reference[i]);

                if (stepDistance > 1)
                {
                    if ((stepDistance + 1) % 3 == 0)
                        result.Insert(0, " ");
                }

                stepDistance++;
            }

            if (currency != null)
                result.Append($" {currency}");

            return result.ToString();
        }
    }
}
