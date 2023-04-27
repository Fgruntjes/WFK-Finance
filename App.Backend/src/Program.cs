using App.Backend.Authentication;
using App.Backend.Authorization;
using App.Backend.Data;
using App.Backend.Nordigen;
using App.Backend.Service;
using App.Backend.SwaggerGen;
using DotNetEnv.Configuration;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv(".local.env");

builder.Services.AddControllers();
builder.Services.AddCors();

builder.Services.AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton(DateTimeZoneProviders.Tzdb);

builder.Services.AddScoped<AuthContext>();
builder.Services.AddScoped<DatabaseContext>();
builder.Services.Scan(scan => scan.FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaceOf<BankConnectService>())
        .AsSelf()
        .WithScopedLifetime());

builder.AddAuth0();
builder.AddSwagger();
builder.AddNordigenClient();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
{
    policy.WithOrigins(builder.Configuration["App:FrontendUrl"] ?? "http://localhost:3000")
        .WithHeaders(
            "Authorization",
            AuthContext.HeaderTenant
        )
        .AllowCredentials()
        .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
