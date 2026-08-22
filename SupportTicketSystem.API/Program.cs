using SupportTicketSystem.API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSystemServices(
    builder.Configuration);

builder.Services.AddApplicationServices(
    builder.Configuration);

var app = builder.Build();

await app.UseApplicationMiddleware();

app.Run();