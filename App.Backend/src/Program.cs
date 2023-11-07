using App.Backend.Startup;
using GraphQL.AspNet.Configuration;
using GraphQL.AspNet.Security;
using App.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
	.AddEnvironmentVariables()
	.AddJsonFile("appsettings.json", true)
	.AddJsonFile("appsettings.local.json", true);

builder.Services.AddAuth(
	builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Missing 'Auth0:Domain' setting."),
	builder.Configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Missing 'Auth0:Audience' setting.")
);
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression(options =>
{
	options.MimeTypes = new[] { "application/json", "application/graphql-response+json" };
});
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		policy =>
		{
			policy.WithOrigins("*")
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

// App configuration
builder.Services.AddDatabase(
	builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Missing 'ConnectionStrings:DefaultConnection' setting."));
builder.Services.AddGraphQL(c =>
	{
		c.AuthorizationOptions.Method = AuthorizationMethod.PerRequest;
		c.ExecutionOptions.ResolverIsolation = ResolverIsolationOptions.All;
		c.ResponseOptions.ExposeExceptions = builder.Environment.IsDevelopment();
	});
builder.Services.AddNordigenClient(builder.Configuration);
builder.Services.AddAppServices();

var app = builder.Build();
app.Logger.LogInformation("Started application with auth0 {domain} and audicence {audience}",
	builder.Configuration["Auth0:Domain"],
	builder.Configuration["Auth0:Audience"]);

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseGraphQL();

app.Run("http://*:8080");

public partial class Program
{

}