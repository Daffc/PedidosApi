using FluentValidation;
using FluentValidation.AspNetCore;


using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Application.Validators.Requests;
using Api.Middlewares;
using Infrastructure.Persistence.Database;

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
builder.Services.AddValidatorsFromAssemblyContaining<GetPedidosRequestValidator>();


var app = builder.Build();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CONFIGURANDO BANCO DE DADOS
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() )
{
    MigrationManager.ApplyMigrations(app.Services);
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }