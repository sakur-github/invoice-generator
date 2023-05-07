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

        TextStyle header1 = TextStyle.Default.FontSize(24).Bold().FontColor("151515").FontFamily("Roboto-Bold");
        TextStyle header2 = TextStyle.Default.FontSize(16).SemiBold().FontColor("151515").FontFamily("Roboto-Medium");
        TextStyle boldText = TextStyle.Default.FontSize(12).SemiBold().FontColor("151515").FontFamily("Roboto-Bold");
        TextStyle regularText = TextStyle.Default.FontSize(12).SemiBold().FontColor("151515").FontFamily("Roboto-Regular");

        const int tablePadding = 10;

        public string FileName
        {
            get
            {
                return $"{(GeneralConfiguration?.Sender?.Name ?? "Invoice").Split()[0]}_{InstanceConfiguration.Number ?? "1"}.pdf";
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
                    page.MarginVertical(40);
                    page.MarginHorizontal(35);

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
            container.PaddingHorizontal(tablePadding).Row(row =>
            {
                if (GeneralConfiguration.LogoUrl != null)
                    row.ConstantItem(64).Height(64).Image(GeneralConfiguration.GetImageBytes(), ImageScaling.Resize);

                row.RelativeItem().AlignLeft().Text($"    {GeneralConfiguration?.Sender?.Name ?? ""}").Style(header1);

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Spacing(-4);

                    column.Item().AlignRight().Text($"INVOICE").Style(header2);

                    column.Item().AlignRight().MinHeight(15);

                    const int rowWidth = 180;

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice number").Style(boldText);
                        row.RelativeItem().AlignRight().Text(InstanceConfiguration.Number).Style(regularText);
                    });

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice date").Style(boldText);
                        row.RelativeItem().AlignRight().Text(InstanceConfiguration.IssueDate.ToString("yyyy-MM-dd")).Style(regularText);
                    });

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice due date").Style(boldText);
                        row.RelativeItem().AlignRight().Text(InstanceConfiguration.IssueDate.AddDays(GeneralConfiguration?.DaysToPay ?? 30).ToString("yyyy-MM-dd")).Style(regularText);
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(-2);

                column.Item().PaddingHorizontal(10).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text("Sender").Style(header2);
                    row.RelativeItem().AlignRight().Text("Recipient").Style(header2);
                });

                column.Item().AlignLeft().MinHeight(10);

                column.Item().PaddingHorizontal(tablePadding).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(GeneralConfiguration?.Sender?.Name ?? "").Style(regularText);
                    row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Receiver?.Name ?? "").Style(regularText);
                });

                column.Item().PaddingHorizontal(tablePadding).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(GeneralConfiguration?.Sender?.Address?.FirstLine ?? "[missing address first line]").Style(regularText);
                    row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Receiver?.Address?.FirstLine ?? "[missing address first line]").Style(regularText);
                });

                column.Item().PaddingHorizontal(tablePadding).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(GeneralConfiguration?.Sender?.Address?.SecondLine ?? "[missing address second line]").Style(regularText);
                    row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Receiver?.Address?.SecondLine ?? "[missing address second line]").Style(regularText);
                });

                column.Item().AlignLeft().MinHeight(30);

                column.Item().Padding(tablePadding).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Column(column =>
                    {
                        column.Item().Text("Sender reference").Style(boldText);
                        column.Item().Text(GeneralConfiguration.Sender?.Reference?.Name ?? "[missing sender reference name]");
                        column.Item().Text(GeneralConfiguration.Sender?.Reference?.Email ?? "[missing sender reference email]");

                        column.Item().AlignRight().MinHeight(20);

                        column.Item().Text("Recipient reference").Style(boldText);
                        column.Item().Text(GeneralConfiguration.Receiver?.Reference?.Name ?? "[missing receiver reference name]");
                        column.Item().Text(GeneralConfiguration.Receiver?.Reference?.Email ?? "[missing receiver reference email]");
                    });

                    row.RelativeItem().AlignRight().Column(column =>
                    {
                        column.Item().AlignRight().MinHeight(20);

                        column.Item().AlignRight().Text("Payment Information").Style(header2);
                        column.Item().AlignRight().MinHeight(8);

                        const int rowWidth = 220;

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("BIC:").Style(boldText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.PaymentInformation?.Bic ?? "[missing paymentInformation bic]").Style(regularText);
                        });

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("IBAN:").Style(boldText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.PaymentInformation?.FormattedIban ?? "[missing paymentInformation iban]").Style(regularText);
                        });

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Organization nr:").Style(boldText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Sender?.OrganizationNumber ?? "[missing sender organization number]").Style(regularText);
                        });
                    });
                });

                column.Item().MinHeight(30);

                column.Item().Background("F5F5F5").Padding(tablePadding).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(15);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn(1.3f);
                        columns.RelativeColumn(1.8f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("#");
                        header.Cell().Element(CellStyle).Text("Description");
                        header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit");
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                        header.Cell().Element(CellStyle).AlignRight().Text("Total");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    if (TimeExport.Times == null)
                        throw new GenerationException("TimeExport is missing times. Can not generate invoice without information about times");

                    foreach (Time item in TimeExport.Times)
                    {
                        if (item.Name == null)
                            throw new GenerationException("Missing name for time entry");

                        int unitPrice = GeneralConfiguration.GetUnitPrice(item.Name);

                        table.Cell().Element(CellStyle).Text((TimeExport.Times.IndexOf(item) + 1).ToString());
                        table.Cell().Element(CellStyle).Text(item.Name);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text("hours");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{unitPrice.ToPriceString("SEK")}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{(unitPrice * item.Amount).ToPriceString("SEK")}");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });

                column.Item().MinHeight(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        const int rowWidth = 270;

                        int totalCost = TimeExport.GetTotalCost(GeneralConfiguration);
                        int totalVat = (int)(totalCost * 0.25);

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Totalt belopp före moms").Style(boldText);
                            row.RelativeItem().AlignRight().Text(totalCost.ToPriceString("SEK")).Style(regularText);
                        });

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Total moms").Style(boldText);
                            row.RelativeItem().AlignRight().Text(totalVat.ToPriceString("SEK")).Style(regularText);
                        });

                        column.Item().MinHeight(10);

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Summa att betala").Style(boldText);
                            row.RelativeItem().AlignRight().Text((totalVat + totalCost).ToPriceString("SEK")).Style(regularText);
                        });
                    });
                });
            });
        }
    }
}
