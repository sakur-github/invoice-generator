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

        TextStyle header1 = TextStyle.Default.FontSize(24).FontColor("151515").FontFamily("Roboto-Bold");
        TextStyle header2 = TextStyle.Default.FontSize(16).FontColor("151515").FontFamily("Roboto-Medium");
        TextStyle boldText = TextStyle.Default.FontSize(12).FontColor("151515").FontFamily("Roboto-Bold");
        TextStyle regularText = TextStyle.Default.FontSize(12).FontColor("151515").FontFamily("Roboto-Regular");

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
                    page.Footer().Element(ComposeFooter);
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

                    column.Item().AlignRight().Text($"INVOICE".ToCorrectLanguage(this)).Style(header2);

                    column.Item().AlignRight().MinHeight(15);

                    const int rowWidth = 180;

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice number".ToCorrectLanguage(this)).Style(boldText);
                        row.RelativeItem().AlignRight().Text(InstanceConfiguration.Number).Style(regularText);
                    });

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice date".ToCorrectLanguage(this)).Style(boldText);
                        row.RelativeItem().AlignRight().Text(InstanceConfiguration.IssueDate.ToString("yyyy-MM-dd")).Style(regularText);
                    });

                    column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Invoice due date".ToCorrectLanguage(this)).Style(boldText);
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
                    row.RelativeItem().AlignLeft().Text("Sender".ToCorrectLanguage(this)).Style(header2);
                    row.RelativeItem().AlignRight().Text("Recipient".ToCorrectLanguage(this)).Style(header2);
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
                        column.Item().Text("Sender reference".ToCorrectLanguage(this)).Style(boldText);
                        column.Item().Text(GeneralConfiguration.Sender?.Reference?.Name ?? "[missing sender reference name]").Style(regularText);
                        column.Item().Text(GeneralConfiguration.Sender?.Reference?.Email ?? "[missing sender reference email]").Style(regularText);

                        column.Item().AlignRight().MinHeight(20);

                        column.Item().Text("Recipient reference".ToCorrectLanguage(this)).Style(boldText);
                        column.Item().Text(GeneralConfiguration.Receiver?.Reference?.Name ?? "[missing receiver reference name]").Style(regularText);
                        column.Item().Text(GeneralConfiguration.Receiver?.Reference?.Email ?? "[missing receiver reference email]").Style(regularText);
                    });

                    row.RelativeItem().AlignRight().Column(column =>
                    {
                        column.Item().AlignRight().MinHeight(20);

                        column.Item().AlignRight().Text("Payment Information".ToCorrectLanguage(this)).Style(header2);
                        column.Item().AlignRight().MinHeight(8);

                        const int rowWidth = 220;

                        if (GeneralConfiguration.Swedish)
                        {
                            column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Text("Bankgiro:").Style(boldText);
                                row.RelativeItem().AlignRight().Text(GeneralConfiguration?.PaymentInformation?.BankgiroNumber ?? "[missing paymentInformation bgnr]").Style(regularText);
                            });
                        }
                        else
                        {
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
                        }

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Organization nr:".ToCorrectLanguage(this)).Style(boldText);
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
                        columns.RelativeColumn(2.8f);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn();
                        columns.RelativeColumn(1.4f);

                        if(GeneralConfiguration.IncludeTax)
                        {
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(2f);
                        }

                        columns.RelativeColumn(1.8f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("#").Style(boldText);
                        header.Cell().Element(CellStyle).Text("Description".ToCorrectLanguage(this)).Style(boldText);
                        header.Cell().Element(CellStyle).AlignRight().Text("Quantity".ToCorrectLanguage(this)).Style(boldText);
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit".ToCorrectLanguage(this)).Style(boldText);
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit price".ToCorrectLanguage(this)).Style(boldText);

                        if(GeneralConfiguration.IncludeTax)
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("VAT%".ToCorrectLanguage(this)).Style(boldText);
                            header.Cell().Element(CellStyle).AlignRight().Text("VAT".ToCorrectLanguage(this)).Style(boldText);
                        }

                        header.Cell().Element(CellStyle).AlignRight().Text("Total".ToCorrectLanguage(this)).Style(boldText);

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

                        table.Cell().Element(CellStyle).Text((TimeExport.Times.IndexOf(item) + 1).ToString()).Style(regularText);
                        table.Cell().Element(CellStyle).Text(item.Name).Style(regularText);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString()).Style(regularText);
                        table.Cell().Element(CellStyle).AlignRight().Text("hours".ToCorrectLanguage(this)).Style(regularText);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{unitPrice.ToPriceString("SEK".ToCorrectLanguage(this))}").Style(regularText);

                        if(GeneralConfiguration.IncludeTax)
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text("25%").Style(regularText);
                            table.Cell().Element(CellStyle).AlignRight().Text($"{(item.Amount * unitPrice * 0.25).ToPriceString("SEK".ToCorrectLanguage(this))}").Style(regularText);
                        }

                        table.Cell().Element(CellStyle).AlignRight().Text($"{(unitPrice * item.Amount).ToPriceString("SEK".ToCorrectLanguage(this))}").Style(regularText);

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
                        int totalVat = GeneralConfiguration.IncludeTax ? (int)(totalCost * 0.25) : 0;

                        if (GeneralConfiguration.IncludeTax)
                        {
                            column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Text("Total before VAT".ToCorrectLanguage(this)).Style(boldText);
                                row.RelativeItem().AlignRight().Text(totalCost.ToPriceString("SEK".ToCorrectLanguage(this))).Style(regularText);
                            });

                            column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Text("Total VAT".ToCorrectLanguage(this)).Style(boldText);
                                row.RelativeItem().AlignRight().Text(totalVat.ToPriceString("SEK".ToCorrectLanguage(this))).Style(regularText);
                            });

                            column.Item().MinHeight(10);
                        }

                        column.Item().AlignRight().MaxWidth(rowWidth).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("Amount to pay".ToCorrectLanguage(this)).Style(boldText);
                            row.RelativeItem().AlignRight().Text((totalVat + totalCost).ToPriceString("SEK".ToCorrectLanguage(this))).Style(regularText);
                        });
                    });
                });

                if(!string.IsNullOrEmpty(InstanceConfiguration.Comment))
                {
                    column.Item().MinHeight(30);

                    column.Item().Background("F5F5F5").Padding(tablePadding).Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().AlignLeft().Text("Comment:".ToCorrectLanguage(this)).Style(boldText);
                            column.Item().AlignLeft().Text(InstanceConfiguration.Comment).Style(regularText);
                        });
                    });
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Row(row =>
                {
                    row.AutoItem().Width(40);

                    row.RelativeItem().AlignRight().Column(column =>
                    {
                        column.Spacing(-3);

                        column.Item().AlignLeft().Text(GeneralConfiguration?.Sender?.Name ?? "[missing sender name]").Style(boldText);
                        column.Item().AlignLeft().Text(GeneralConfiguration?.Sender?.Address?.FirstLine ?? "[missing sender address first line]").Style(regularText);
                        column.Item().AlignLeft().Text(GeneralConfiguration?.Sender?.Address?.SecondLine ?? "[missing sender address second line]").Style(regularText);
                        column.Item().AlignLeft().Text($"Org. nr. {GeneralConfiguration?.Sender?.OrganizationNumber ?? "[missing sender organization number]"}").Style(regularText);
                    });

                    row.AutoItem().Width(40);

                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(-3);

                        column.Item().AlignLeft().Text("Contact information".ToCorrectLanguage(this)).Style(boldText);

                        column.Item().AlignLeft().Row(row =>
                        {
                            row.AutoItem().AlignLeft().Text("Name:".ToCorrectLanguage(this)).Style(regularText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Sender?.Reference?.Name ?? "[missing sender reference name]").Style(regularText);
                        });

                        column.Item().AlignLeft().Row(row =>
                        {
                            row.AutoItem().AlignLeft().Text("Phone:".ToCorrectLanguage(this)).Style(regularText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Sender?.Reference?.PhoneNumber ?? "[missing sender reference phone number]").Style(regularText);
                        });

                        column.Item().AlignLeft().Row(row =>
                        {
                            row.AutoItem().AlignLeft().Text("Email:".ToCorrectLanguage(this)).Style(regularText);
                            row.RelativeItem().AlignRight().Text(GeneralConfiguration?.Sender?.Reference?.Email ?? "[missing sender reference email]").Style(regularText);
                        });
                    });

                    row.AutoItem().Width(100);
                });
            });
        }
    }
}
