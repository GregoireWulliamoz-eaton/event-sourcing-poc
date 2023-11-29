using EventSourcing;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Mvc;
using Weasel.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
    
    options.Projections.Snapshot<ChargerConfiguration>(SnapshotLifecycle.Inline);
}).UseLightweightSessions();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/Charger/Configuration/{chargerId}",
    async (Guid chargerId, ConfigurationChanged configurationChanged, [FromServices] IDocumentSession session) =>
    {
        session.Events.Append(chargerId, configurationChanged);

        await session.SaveChangesAsync();
    });

app.MapGet("/Charger/Configuration/{chargerId}",
    async (Guid chargerId, long? version, [FromServices] IDocumentSession session) =>
    {
        if (version is null)
        {
            return await session.Events.AggregateStreamAsync<ChargerConfiguration>(chargerId);
        }
        
        return await session.Events.AggregateStreamAsync<ChargerConfiguration>(chargerId, version.Value);
        
    });

app.Run();