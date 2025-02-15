using SupplyChainManagement.Application;
using SupplyChainManagement.Application.Extensions;
using SupplyChainManagement.Persistence.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddPersistenceExtensions(builder.Configuration).AddApplicationExtension(typeof( ApplicationAssembly));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
