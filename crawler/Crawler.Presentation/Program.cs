using Crawler.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Crawler API", Version = "v1.0.0"
        }));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:8080", "http://localhost:8081")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


DependencyContainer.AddDbContext(builder.Services, builder.Configuration, "CrawlDB");
DependencyContainer.AddServices(builder.Services);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

DependencyContainer.EnsureMigration(app.Services);

app.MapControllers();

app.UseCors();

app.Run();
