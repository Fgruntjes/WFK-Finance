using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Nordigen;
using App.Backend.Service;
using DotNetEnv.Configuration;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddDotNetEnv(".local.env");
}

var auht0Settings = builder.Configuration.GetSection("Auth0").Get<Auth0Options>();
if (auht0Settings == null) throw new Exception("Auth0 settings not found");

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerDocument();

builder.Services.AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton(DateTimeZoneProviders.Tzdb);
builder.Services.AddScoped<AuthContext>();
builder.Services.AddScoped<DatabaseContext>();
builder.Services.Scan(scan => scan.FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaceOf<InstitutionConnectionService>())
        .AsSelf()
        .WithTransientLifetime());

builder.AddAuth0(auht0Settings);
builder.AddNordigenClient();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseCors(policy =>
{
    policy.WithOrigins(builder.Configuration["App:FrontendUrl"] ?? "http://localhost:3000")
        .WithHeaders(
            "Authorization",
            AuthContext.TenantHeader,
            "Content-Type"
        )
        .AllowCredentials()
        .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program
{ }
