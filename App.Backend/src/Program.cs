using App.Backend.Authentication;
using App.Backend.Authorization;
using App.Backend.SwaggerGen;
using DotNetEnv.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv(".local.env");

builder.Services.AddControllers();
builder.Services.AddCors();
builder.AddAuth0();
builder.AddSwagger();

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
