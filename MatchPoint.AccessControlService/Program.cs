using System.Runtime.CompilerServices;
using Asp.Versioning;
using MatchPoint.AccessControlService.Configuration;

// Necessary to run integration tests with mock host
[assembly: InternalsVisibleTo("MatchPoint.AccessControlService.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Service custom services
builder.AddAccessControlServices();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
