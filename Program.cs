using Routes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//
// App endpoints 
//

ProductEndpoints.Map(app);
UserEndpoints.Map(app);

//
// App configuration
//

app.UseHttpsRedirection();


app.Run();

