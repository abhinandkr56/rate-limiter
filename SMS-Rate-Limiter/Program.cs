using MongoDB.Driver;
using SMS_Rate_Limiter.Models;
using SMS_Rate_Limiter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings.ConnectionString));


builder.Services.AddTransient<IRateLimitService, RateLimitService>();
builder.Services.AddTransient<IMongoService, MongoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.MapControllers(); 

app.Run();
