using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(opt => {
opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using  var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
     var context =services.GetRequiredService<DataContext>();
      await context.Database.MigrateAsync();
      await Seed.SeedData(context);

}
catch (Exception ex)
{
    var context = services.GetRequiredService<ILogger<Program>>();
    context.LogError(ex,"error while creating the database");

}
app.Run();
