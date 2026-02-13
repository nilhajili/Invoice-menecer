using WebApplication4.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Services;
using WebApplication4.Services.Interfaces;
using WebApplication4.Mapping;
using WebApplication4.DTOs;
using WebApplication4.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

// Add DbContext
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(ConnectionString)
);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Add FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>());

// Swagger / OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();