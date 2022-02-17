using MongoDB.Driver;
using proxyApiABR;
using proxyApiABR.Middleware;
using proxyApiABR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MongoDatabaseSettings"));
builder.Services.AddSingleton<MongoCrudService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ProxyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run(); ///AIzaSyDs22S0v_flzegJFLa3bhULw59o0dCN6WM
