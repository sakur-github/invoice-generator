using InvoiceGenerator.Manager;
using InvoiceGenerator.Models;
using InvoiceGenerator.Models.Configuration;
using InvoiceGenerator.Models.Data;
using QuestPDF.Fluent;
using System.Diagnostics;

namespace InvoiceGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                args = new string[] { "Clockify_Time_Report_Summary_01_03_2023-31_03_2023.csv" };
            }

            try
            {
                string result = Run(args[0]);

                if (result != string.Empty)
                {
                    Console.WriteLine(result);
                    Thread.Sleep(2000);
                }
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

            TimeExport timeExport = TimeExport.FromCsv(File.ReadAllText(clockifyExportPath));

            Invoice invoice = manager.CreateInvoice(instanceConfiguration, timeExport);
            invoice.GeneratePdf(invoice.FileName);

            OpenFile(invoice.FileName);

            return string.Empty;
        }

        private static void OpenFile(string filename)
        {
            Process process = new Process();

            process.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };

            process.Start();
        }
    }
}