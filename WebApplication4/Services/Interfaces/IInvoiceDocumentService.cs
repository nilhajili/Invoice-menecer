namespace WebApplication4.Services.Interfaces;

public interface IInvoiceDocumentService
{
    Task<(Stream stream, string fileName, string contentType)?> 
        GenerateDownloadAsync(Guid invoiceId, string format, CancellationToken cancellationToken = default);
}