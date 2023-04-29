using InvoiceGenerator.Models.Configuration;

namespace InvoiceGenerator.Models
{
    internal class Invoice
    {
        internal string InvoiceNumber { get; set; }
        internal DateTime InvoiceDate { get; set; }
        internal InvoiceConfiguration Configuration { get; set; }

        public Invoice(string invoiceNumber, DateTime invoiceDate, InvoiceConfiguration configuration) 
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Configuration = configuration;
        }
    }
}
