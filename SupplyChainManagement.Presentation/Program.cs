using DotNetEnv;
using SupplyChainManagement.Application;
using SupplyChainManagement.Application.Extensions;
using SupplyChainManagement.Persistence.Extensions;
using SupplyChainManagement.Presentation.ExceptionHandlers;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddPersistenceExtensions(builder.Configuration).AddApplicationExtension(typeof( ApplicationAssembly));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
