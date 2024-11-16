using AgroGestor360Client.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace AgroGestor360Client.Tools.ReportsTemplate;

public class SaleDocument : IDocument
{
    class Report
    {
        public string? Name { get; set; }
        public int Absence { get; set; }
        public double TotalCollected { get; set; }
    }

    readonly SaleReport data;

    public SaleDocument(SaleReport model)
    {
        data = model;
        data.SaleItems = [.. model.SaleItems!.OrderBy(x => x.SaleEntryDate)];
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
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Black));

                page.Header().PaddingBottom(10).Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().MaxHeight(30).Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(15);
            column.Item().AlignCenter().Text("REPORTE DE VENTAS").FontSize(11).SemiBold();
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
                        text.Span($"{data.IssueDate:dd MMM yyyy}");
                    });
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("TIPO: ").SemiBold();
                        text.Span($"{data.SaleState}");
                    });
                    c.Item().AlignRight().Text(text =>
                    {
                        text.Span("ORDEN: ").SemiBold();
                        text.Span($"{data.OrderBy}");
                    });
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(cd =>
            {
                cd.ConstantColumn(60);
                cd.RelativeColumn(65);
                cd.ConstantColumn(100);
                cd.ConstantColumn(100);
                cd.ConstantColumn(60);
                cd.ConstantColumn(90);
                cd.ConstantColumn(90);
            });

            table.Header(h =>
            {
                h.Cell().Element(CellStyle).Text("FECHA DE VENTA");
                h.Cell().Element(CellStyle).Text("NO. FACTURA");
                h.Cell().Element(CellStyle).Text("VENDEDOR");
                h.Cell().Element(CellStyle).Text("CLIENTE");
                h.Cell().Element(CellStyle).Text("ULTIMO ABONO");
                h.Cell().Element(CellStyle).Text("ESTADO/ABONO");
                h.Cell().Element(CellStyle).Text("IMPORTE");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderHorizontal(1).PaddingVertical(2).DefaultTextStyle(ts => ts.SemiBold()).AlignCenter().AlignMiddle().BorderColor(Colors.Grey.Lighten2);
                }
            });

            foreach (var item in data.SaleItems!)
            {
                table.Cell().Element(CellStyle).AlignLeft().Text(item.SaleEntryDate.ToString("dd/MM/yyyy"));
                table.Cell().Element(CellStyle).Text(item.Code);
                table.Cell().Element(CellStyle).ExtendHorizontal().Text(item.Seller);
                table.Cell().Element(CellStyle).ExtendHorizontal().Text(item.Customer);
                table.Cell().Element(CellStyle).AlignCenter().Text(item.SaleDate?.ToString("dd/MM/yyyy") ?? "-");
                switch (item.SaleStatus)
                {
                    case SaleStatus.Paid:
                        table.Cell().Element(CellStyle).AlignCenter().Text("PAGADA");
                        break;
                    case SaleStatus.Cancelled:
                        table.Cell().Element(CellStyle).AlignCenter().Text("CANCELADA");
                        break;
                    default:
                        if (item.TotalPaid == 0)
                        {
                            table.Cell().Element(CellStyle).AlignCenter().Text("-");
                        }
                        else
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text(item.TotalPaid.ToString("F2"));
                        }
                        break;
                }
                table.Cell().Element(CellStyle).AlignRight().Text(item.TotalToPay.ToString("F2"));

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).PaddingHorizontal(2).DefaultTextStyle(dts => dts.FontSize(8)).AlignTop();
                }
            }

            table.Footer(f =>
            {
                f.Cell().ColumnSpan(5).Element(CellStyle).Text("TOTAL").SemiBold();
                f.Cell().Element(CellStyle).AlignRight().Text(data.SaleItems.Sum(x => x.TotalPaid).ToString("C"));
                f.Cell().Element(CellStyle).AlignRight().Text(data.SaleItems.Sum(x => x.TotalToPay).ToString("C"));

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderHorizontal(1).PaddingVertical(2).AlignMiddle();
                }
            });
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
        });
    }
}
