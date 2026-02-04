using WebApplication4.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Services;
using WebApplication4.Services.Interfaces;
using WebApplication4.Mapping;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<InvoiceDbContext>(
    options => options.UseSqlServer(ConnectionString)
);
builder.Services.AddAutoMapper(typeof ( MappingProfile ));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

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