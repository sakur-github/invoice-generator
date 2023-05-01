using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Models.Data
{
    public class Time
    {
        public string? Name { get; set; }
        public string? Project { get; set; }
        public string? Client { get; set; }
        public int Amount { get; set; }
    }
}
