using App.Backend.Startup;
using GraphQL.AspNet.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddAuthorization()
	.AddAuthentication()
	.AddJwtBearer();
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
builder.Services.AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing 'ConnectionStrings.DefaultConnection' setting."));
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