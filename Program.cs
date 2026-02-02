using Routes;
using Data;
using Entities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

//
// App endpoints 
//

app.MapGet("/", () =>
{
    return "Hello world";
});

ProductEndpoints.Map(app);
UserEndpoints.Map(app);

//
// App configuration
//

app.UseHttpsRedirection();



app.Run();

