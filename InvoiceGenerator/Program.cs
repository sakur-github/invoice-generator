using InvoiceGenerator.Models.Configuration;

namespace InvoiceGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllText("InvoiceConfiguration.json", InvoiceConfiguration.Default.ToJson());

            Console.WriteLine("Hello, World!");
        }
    }
}