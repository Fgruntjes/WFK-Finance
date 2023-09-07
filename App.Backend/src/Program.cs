using App.Backend.Startup;
using GraphQL.AspNet.Configuration;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddLogging();
builder.Services.AddDatabase(
	builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Missing 'ConnectionStrings:DefaultConnection' setting."));
builder.Services.AddGraphQL();

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