using PiorEcommerceDoMundo.Infrastructure;
using PiorEcommerceDoMundo.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // TODO: remover quando todas as respostas usarem DTOs (problema 5)
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
// Cada dev registra aqui o seu dominio com UMA linha, ex: builder.Services.AddProdutos();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();