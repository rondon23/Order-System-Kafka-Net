using Order.API.Kafka;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Services
// ===============================

// Controllers (necessário para OrdersController)
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kafka Producer (Singleton)
builder.Services.AddSingleton<KafkaProducer>();

// ===============================
// App
// ===============================

var app = builder.Build();

// ===============================
// Middleware
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();