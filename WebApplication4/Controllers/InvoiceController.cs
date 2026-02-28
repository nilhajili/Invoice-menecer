using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using WebApplication4.Models;
using WebApplication4.Services.Interfaces;
using WebApplication4.DTOs;

namespace WebApplication4.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly IMapper _mapper;
    private readonly IInvoiceDocumentService _invoiceDocumentService;

    public InvoiceController(IInvoiceDocumentService invoiceDocumentService, IInvoiceService invoiceService)
    {
        _invoiceDocumentService = invoiceDocumentService;
        _service = invoiceService;
    }
    
    public InvoiceController(IInvoiceService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }
    

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceDto dto)
    {
        var invoice = _mapper.Map<Invoice>(dto);
        invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        var created = await _service.CreateAsync(invoice);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<InvoiceDto>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        var invoice = _mapper.Map<Invoice>(dto);
        invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        var updated = await _service.UpdateAsync(id, invoice);
        if (updated == null)
            return BadRequest("Only non-sent invoices can be updated.");
        return Ok(_mapper.Map<InvoiceDto>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await _service.HardDeleteAsync(id);
        if (!result)
            return BadRequest("Only non-sent invoices can be deleted.");
        return NoContent();
    }

    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await _service.ArchiveAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] InvoiceQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(_mapper.Map<PagedResult<InvoiceResponseDto>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
    {
        var invoice = await _service.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();
        return Ok(_mapper.Map<InvoiceDto>(invoice));
    }
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] InvoiceStatusDto dto)
    {
        var succeeded = await _service.ChangeStatusAsync(User, id, dto.Status);

        if (!succeeded)
            return Forbid();

        return NoContent();
    }
    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadInvoice(Guid id, string format = "pdf")
    {
        var result = await _invoiceDocumentService.GenerateDownloadAsync(id, format);

        if (result is null)
            return NotFound();

        var invoice = await _service.GetInvoiceByIdAsync(id);
        if (invoice.CreatedByUserId != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            return Forbid();

        return File(result.Value.stream, result.Value.contentType, result.Value.fileName);
    }
}
