using FluentValidation;
using FluentValidation.AspNetCore;


using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Application.Validators.Requests;
using Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ADICIONANDO CONTROLLERS
builder.Services.AddControllers();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// APPLICATION
builder.Services.AddApplication();

// INFRASTRUCTURE
builder.Services.AddInfrastructure(builder.Configuration);

// FLUENTVALIDATION
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePedidoRequestValidator>();


var app = builder.Build();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }