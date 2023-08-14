using App.Backend.Data;
using App.Backend.GraphQL;
using GraphQL;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression(options =>
{
	options.MimeTypes = new[] { "application/json", "application/graphql-response+json" };
});

// Database connection
builder.Services.AddEntityFrameworkNpgsql()
	.AddDbContext<DatabaseContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// App services
builder.Services.AddSingleton<AppQuery>();

builder.Services.AddGraphQL(b => b
	.AddSchema<AppSchema>()
	.AddNewtonsoftJson());
// @see https://github.com/graphql-dotnet/graphql-dotnet/issues/1116
builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.AllowSynchronousIO = true;
});

var app = builder.Build();
app.UseResponseCompression();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseGraphQLAltair();
app.UseGraphQL();

app.Run();