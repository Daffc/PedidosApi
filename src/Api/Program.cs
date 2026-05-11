using FluentValidation;

using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Application.Validators.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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
builder.Services.AddValidatorsFromAssemblyContaining<CreatePedidoRequestValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
