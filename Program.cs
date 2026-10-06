using RoutingDemo.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Регистриране на услуги (dependency injection) ---
builder.Services.AddControllers();        // Активира контролерите (атрибут-роутинга)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();         // Swagger: /swagger

// In-memory хранилището се подава в контролера като singleton
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();

// --- HTTP request pipeline ---
// В Development режим включваме Swagger UI за интерактивно тестване
// на GET / POST / DELETE заявките на http://localhost:<port>/swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Сервира статичните файлове от wwwroot/ (нашия HTML/JS frontend).
// UseDefaultFiles прави така, че "/" да зарежда wwwroot/index.html.
app.UseDefaultFiles();
app.UseStaticFiles();

// MapControllers активира откриването на маршрутите от атрибутите
// [Route], [HttpGet], [HttpPost], [HttpDelete] в контролерите.
app.MapControllers();

app.Run();

