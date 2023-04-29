namespace InvoiceGenerator.Models.Data
{
    internal class Company
    {
        public string? Name { get; set; }
        public Address? Address { get; set; }
        public string? OrganizationNumber { get; set; }
        public Person? Reference { get; set; }
    }
}
