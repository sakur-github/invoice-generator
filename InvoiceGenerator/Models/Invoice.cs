using InvoiceGenerator.Models.Configuration;

namespace InvoiceGenerator.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public InvoiceConfiguration Configuration { get; set; }

        public Invoice(string invoiceNumber, DateTime invoiceDate, InvoiceConfiguration configuration) 
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            Configuration = configuration;
        }
    }
}
