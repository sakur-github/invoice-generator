using InvoiceGenerator.Manager;
using InvoiceGenerator.Models.Configuration;

namespace InvoiceGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string result = Run(args[0]);

                Console.WriteLine(result);
                Thread.Sleep(2000);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.ReadKey();
            }
        }

        private static string Run(string clockifyExportPath)
        {
            InvoiceManager manager = new InvoiceManager("InvoiceConfiguration.json");

            if (manager.Configuration == InvoiceConfiguration.Default)
                throw new ArgumentException("InvoiceConfiguration.json must be modified");

            InvoiceInstanceConfiguration instanceConfiguration = manager.GetInvoiceInstance("InvoiceInstance.json");

            if (!File.Exists(clockifyExportPath))
                throw new ArgumentException($"The following provided clockify export path could not be read from {clockifyExportPath}");

            throw new NotImplementedException();
        }
    }
}