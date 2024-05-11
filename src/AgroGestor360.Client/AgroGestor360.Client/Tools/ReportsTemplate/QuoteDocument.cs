using AgroGestor360.Client.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace AgroGestor360.Client.Tools.ReportsTemplate;

public class QuoteDocument : IDocument
{
    class Report
    {
        public string? Name { get; set; }
        public int Absence { get; set; }
        public double TotalCollected { get; set; }
    }

    readonly CustomerQuoteReport data;

    public QuoteDocument(CustomerQuoteReport model)
    {
        data = model;
    }

    public DocumentMetadata GetMetaData() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.Letter);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("VERDANA").FontColor(Colors.Black));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().MaxHeight(30).Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(15);
            column.Item().AlignCenter().Text("COTIZACION").FontSize(11).SemiBold();
            column.Item().AlignLeft().Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(text =>
                    {
                        text.Span("EMPRESA: ").SemiBold();
                        text.Span($"{data.OrganizationName}");
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("TELÉFONO: ").SemiBold();
                        text.Span($"{data.OrganizationPhone}");
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("CORREO ELECTRÓNICO: ").SemiBold();
                        text.Span($"{data.OrganizationEmail}");
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("DIRECCIÓN: ").SemiBold();
                        text.Span($"{data.OrganizationAddress}");
                    });
                });

                row.ConstantItem(250).Column(c =>
                {
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("EMISIÓN: ").SemiBold();
                        text.Span($"{data.QuotationDate:dd MMM yyyy}");
                    });
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("VENCIMIENTO: ").SemiBold();
                        text.Span($"{data.QuotationDate.AddDays(2):dd MMM yyyy}");
                    });
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("VENDEDOR: ").SemiBold();
                        text.Span($"{data.SellerName}");
                    });
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("IMPORTE: ").SemiBold();
                        text.Span($"{data.TotalAmount:N2}");
                    });
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(c =>
        {
            c.Item().PaddingTop(10).Text(text =>
            {
                text.Span("CLIENTE: ").SemiBold();
                text.Span($"{data.CustomerName}");
            });
            c.Item().Text(text =>
            {
                text.Span("TELÉFONO: ").SemiBold();
                text.Span($"{data.CustomerPhone}");
            });
            c.Item().PaddingTop(10).Element(ComposeTable);
        });
    }

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(32);
                c.RelativeColumn();
                c.ConstantColumn(75);
                c.ConstantColumn(100);
            });

            table.Header(h =>
            {
                h.Cell().Element(CellStyle).Text("NO.");
                h.Cell().Element(CellStyle).Text("DESCRIPCIÓN");
                h.Cell().Element(CellStyle).Text("CANTIDAD UNITARIA");
                h.Cell().Element(CellStyle).Text("COSTO");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderHorizontal(1).PaddingVertical(2).DefaultTextStyle(ts => ts.SemiBold()).AlignCenter().AlignMiddle().BorderColor(Colors.Grey.Lighten2);
                }
            });
            int count = 1;
            foreach (var item in data.ProductItems!)
            {
                table.Cell().Element(CellStyle).AlignCenter().Text(count.ToString());
                table.Cell().Element(CellStyle).ExtendHorizontal().AlignLeft().Text(item.ProductName);
                table.Cell().Element(CellStyle).AlignRight().Text(item.ProductQuantity.ToString("F2"));
                table.Cell().Element(CellStyle).AlignRight().Text(item.ArticlePrice.ToString("N2"));
                count++;

                static IContainer CellStyle(IContainer container)
                {
                    return container.AlignMiddle().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
                }
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(150)
                .ExtendVertical()
                .Text(t =>
                {
                    t.Span("Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });

            row.RelativeItem().AlignRight().Text($"No: {data.QuotationCode}").SemiBold();
        });
    }
}
