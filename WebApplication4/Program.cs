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
using Microsoft.AspNetCore.Authorization;
using WebApplication4.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>());


builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TaskStatusChangePolicy", policy =>
        policy.Requirements.Add(new TaskStatusChangeRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, TaskStatusChangeHandler>();

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