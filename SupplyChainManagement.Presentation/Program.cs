using DotNetEnv;
using SupplyChainManagement.Application;
using SupplyChainManagement.Application.Extensions;
using SupplyChainManagement.Persistence.Extensions;
using SupplyChainManagement.Presentation.ExceptionHandlers;
using SupplyChainManagement.Presentation.Security;
using System.Text.Json.Serialization;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddPersistenceExtensions(builder.Configuration).AddApplicationExtension(typeof( ApplicationAssembly));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.SeedAdminAsync();

app.Run();
