using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WebApplication4.Models;
using WebApplication4.Services.Interfaces;
using WebApplication4.DTOs;

namespace WebApplication4.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IMapper _mapper;

    public CustomerController(ICustomerService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _service.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    [HttpGet("{id:guid}")] 
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        var created = await _service.CreateAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<CustomerDto>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        var updated = await _service.UpdateAsync(id, customer);
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<CustomerDto>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await _service.HardDeleteAsync(id);
        if (!result) return BadRequest("Cannot delete customer with existing invoices.");
        return NoContent();
    }

    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await _service.ArchiveAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
