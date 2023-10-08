using App.Backend.Startup;
using GraphQL.AspNet.Configuration;
using DotNetEnv.Configuration;
using GraphQL.AspNet.Security;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddDotNetEnv(".local.env");
}

builder.Configuration
	.AddEnvironmentVariables()
	.AddJsonFile("appsettings.json", true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

builder.Services.AddAuth(
	builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Missing 'Auth0:Domain' setting.")
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

app.Run();