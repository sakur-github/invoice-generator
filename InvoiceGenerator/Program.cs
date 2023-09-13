using InvoiceGenerator.Helpers;
using InvoiceGenerator.Manager;
using InvoiceGenerator.Models;
using InvoiceGenerator.Models.Configuration;
using InvoiceGenerator.Models.Data;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using System.Diagnostics;

namespace InvoiceGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "augusti.csv" };

            if (args.Length == 0)
            {
                Console.WriteLine("No time report was provided. Please drag and drop the time report on the .exe file to create an invoice from it.");
                Console.ReadKey();
            }
            else
            {
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

            SetupFont("Roboto", "Regular", "Medium", "Bold").Wait();

            Invoice invoice = manager.CreateInvoice(instanceConfiguration, timeExport);
            invoice.GeneratePdf(invoice.FileName);

            OpenFile(invoice.FileName);

            return string.Empty;
        }

        private static async Task SetupFont(string name, params string[] variations)
        {
            FontHelper font = await FontHelper.FromFontFamilyNameAsync(name);

            foreach (string variation in variations)
            {
                byte[]? fontBytes = await font.GetVariationAsync(variation);

                if (fontBytes == null)
                    throw new Exception($"Font not found: {variation}");

                using (MemoryStream stream = new MemoryStream(fontBytes))
                    FontManager.RegisterFontWithCustomName($"{name}-{variation}", stream);
            }
        }

        private static void OpenFile(string filename)
        {
            Process process = new Process();

            process.StartInfo = new ProcessStartInfo(filename) { UseShellExecute = true };

            process.Start();
        }
    }
}