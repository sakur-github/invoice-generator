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
    }
}
