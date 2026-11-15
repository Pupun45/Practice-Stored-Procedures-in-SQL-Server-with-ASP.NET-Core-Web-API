
var builder = WebApplication.CreateBuilder(args);

// 1. Register Controllers
builder.Services.AddControllers();

// 2. Register OpenAPI (optional)
builder.Services.AddOpenApi();

// 3. Build Application
var app = builder.Build();

// 4. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 5. Map Controller Routes
app.MapControllers();

// 6. Run Application
app.Run();
