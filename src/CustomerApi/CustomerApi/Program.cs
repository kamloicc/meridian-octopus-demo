var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Health endpoint
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow
    });
})
.WithName("GetHealth")
.WithOpenApi();

// Version endpoint
app.MapGet("/version", () =>
{
    var environment = Environment.GetEnvironmentVariable("MERIDIAN_ENVIRONMENT") ?? "Development";
    var version = Environment.GetEnvironmentVariable("MERIDIAN_VERSION") ?? "1.0.0";
    
    return Results.Ok(new
    {
        service = "Meridian Customer API",
        environment = environment,
        version = version,
        timestamp = DateTime.UtcNow
    });
})
.WithName("GetVersion")
.WithOpenApi();

app.Run();
