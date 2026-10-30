using Unimag.Inventario.Api.Common.Middleware;
using Unimag.Inventario.Api.Common.Tenancy;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Application;
using Unimag.Inventario.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Each layer wires up its own dependencies via an extension method.
// Api never new()s a repository, a DbContext or a use case directly.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

// Registered first so it wraps every other middleware and controller —
// any exception thrown anywhere downstream is caught and mapped here.
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
