using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using WebApplication4.Models;
using WebApplication4.Services.Interfaces;
using Document = QuestPDF.Fluent.Document;

namespace WebApplication4.Services;

public class InvoiceDocumentService : IInvoiceDocumentService
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceDocumentService(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<(Stream stream, string fileName, string contentType)?>
        GenerateDownloadAsync(Guid invoiceId, string format, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null)
            return null;

        if (format.ToLower() == "pdf")
        {
            var bytes = GeneratePdf(invoice);
            return (new MemoryStream(bytes),
                $"invoice-{invoice.Id}.pdf",
                "application/pdf");
        }

        if (format.ToLower() == "docx")
        {
            var bytes = GenerateDocx(invoice);
            return (new MemoryStream(bytes),
                $"invoice-{invoice.Id}.docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        return null;
    }
    public  byte[] GeneratePdf(Invoice invoice)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Invoice")
                        .FontSize(22)
                        .Bold();

                    col.Item().Text($"Invoice ID: {invoice.Id}");
                    col.Item().Text($"Customer: {invoice.Customer.Name}");
                    col.Item().Text($"Period: {invoice.StartDate:dd.MM.yyyy} - {invoice.EndDate:dd.MM.yyyy}");
                    col.Item().Text($"Status: {invoice.Status}");
                
                    col.Item().PaddingVertical(10);

                    col.Item().Text("Services:").Bold();

                    foreach (var row in invoice.Rows)
                    {
                        col.Item().Text(
                            $"{row.Service} | Qty: {row.Quantity} | Price: {row.Amount} | Sum: {row.Sum}");
                    }

                    col.Item().PaddingVertical(10);

                    col.Item().Text($"TOTAL: {invoice.TotalSum} AZN")
                        .FontSize(16)
                        .Bold();
                });
            });
        }).GeneratePdf();
    }
    
    public  byte[] GenerateDocx(Invoice invoice)
    {
        using var stream = new MemoryStream();

        using (var wordDocument =
               WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            var body = new Body();

            body.Append(new Paragraph(new Run(new Text("Invoice"))));
            body.Append(new Paragraph(new Run(new Text($"Invoice ID: {invoice.Id}"))));
            body.Append(new Paragraph(new Run(new Text($"Customer: {invoice.Customer.Name}"))));
            body.Append(new Paragraph(new Run(new Text($"Total: {invoice.TotalSum} AZN"))));

            body.Append(new Paragraph(new Run(new Text("Services:"))));

            foreach (var row in invoice.Rows)
            {
                body.Append(new Paragraph(new Run(
                    new Text($"{row.Service} - {row.Sum} AZN")
                )));
            }

            mainPart.Document.Append(body);
            mainPart.Document.Save();
        }

        return stream.ToArray();
    }
}