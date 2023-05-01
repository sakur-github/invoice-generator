using InvoiceGenerator.Helpers;
using InvoiceGenerator.Models.Configuration;
using InvoiceGenerator.Models.Data;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace InvoiceGenerator.Models
{
    public class Invoice : IDocument
    {
        public InvoiceInstanceConfiguration InstanceConfiguration { get; set; }
        public InvoiceConfiguration GeneralConfiguration { get; set; }
        public TimeExport TimeExport { get; set; }

        public DateTime DueDate => InstanceConfiguration.IssueDate.AddDays(GeneralConfiguration.DaysToPay);

        public string FileName
        {
            get
            {
                return $"{GeneralConfiguration?.Sender?.Name ?? "Invoice"}_{InstanceConfiguration.Number ?? "1"}.pdf";
            }
        }

        public Invoice(InvoiceInstanceConfiguration instanceConfiguration, InvoiceConfiguration configuration, TimeExport timeExport)
        {
            GeneralConfiguration = configuration;
            InstanceConfiguration = instanceConfiguration;
            TimeExport = timeExport;
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);


                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            TextStyle header1 = TextStyle.Default.FontSize(24).SemiBold().FontColor("151515").FontFamily("Roboto");
            TextStyle header2 = TextStyle.Default.FontSize(16).SemiBold().FontColor("151515");
            TextStyle boldText = TextStyle.Default.FontSize(12).SemiBold().FontColor("151515");

            container.Row(row =>
            {
                row.ConstantItem(100).Height(50).Placeholder();

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text($"Invoice #{InstanceConfiguration.Number}").Style(header1);

                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").SemiBold();
                        text.Span($"{InstanceConfiguration.IssueDate.ToDateString()}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Due date: ").SemiBold();
                        text.Span($"{DueDate.ToDateString()}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container
                .PaddingVertical(40)
                .Height(250)
                .Background(Colors.Grey.Lighten3)
                .AlignCenter()
                .AlignMiddle()
                .Text("Content").FontSize(16);
        }
    }
}
