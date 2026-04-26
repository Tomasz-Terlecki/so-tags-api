using SoTags.Domain.Queries;
using SoTags.Domain.Commands;
using SoTags.DataProvider.Providers;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Interfaces.Repositories;
using SoTags.Repo;
using SoTags.Repo.Repositories;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add SQL Server DbContext
builder.Services.AddDbContext<SoTagDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISoTagRepository, SoTagRepository>();

builder.Services.AddHttpClient<ISoTagProvider, SoTagProvider>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "SoTagsApp/1.0");
    });

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(GetSoTagsQuery).Assembly);
});

var app = builder.Build();

// Apply EF Core migrations and create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SoTagDbContext>();
    await db.Database.MigrateAsync();
}

// Refetch tags on application startup
using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new RefetchTagsCommand { Count = 1000 });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
