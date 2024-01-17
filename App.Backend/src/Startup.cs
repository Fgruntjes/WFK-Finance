using System.Text.Json.Serialization;
using App.Backend.Mvc;
using App.Backend.OpenApi;
using App.Lib.Configuration;
using App.Lib.Configuration.Options;
using App.Lib.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace App.Backend;

public partial class Startup
{
    private IHostEnvironment Environment { get; }

    public Startup(IHostEnvironment environment)
    {
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
            options.Filters.Add(new GridifyMapperExceptionFilter());
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.UseInlineDefinitionsForEnums();
            config.ParameterFilter<LowerCaseParameterFilter>();
            config.ParameterFilter<GridifyFilterParameterFilter>();
            config.OperationFilter<RangeOperationFilter>();
            config.SchemaFilter<NonNullableFilter>();
            config.SchemaFilter<DtoSuffixFilterFilter>();
            config.CustomSchemaIds(type => type.Name.EndsWith("Dto") ? type.Name[..^3] : type.Name);
            config.TagActionsBy(ApiGroupTagger.GetTags);
        });
        services.AddProblemDetails();
        services.AddResponseCompression();

        var corsPolicy = new CorsPolicyBuilder();
        services.PostConfigure<AppOptions>(appOptions =>
        {
            corsPolicy
                .WithOrigins(appOptions.FrontendUrl.TrimEnd('/'))
                .WithExposedHeaders("Content-Range")
                .WithMethods("GET", "POST", "DELETE", "PUT", "PATCH")
                .WithHeaders("Authorization", "Content-Type", "Range");
        });
        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(corsPolicy.Build());
        });

        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddScoped<IOrganisationIdProvider, HttpContextOrganisationIdProvider>();
    }

    public void Configure(IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        var authOptions = app.ApplicationServices.GetRequiredService<IOptions<AuthOptions>>().Value;
        var appOptions = app.ApplicationServices.GetRequiredService<IOptions<AppOptions>>().Value;
        logger.LogInformation("Started application version: {Version} environment: {Environment}",
            appOptions.Version, appOptions.Environment);
        logger.LogInformation("Auth0 {Domain} and audience {Audience}",
            authOptions.Domain, authOptions.Audience);
        logger.LogInformation("Frontend URL {AppFrontendUrl}",
            appOptions.FrontendUrl);

        app.UseCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseResponseCompression();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseAppHealthChecks();

        if (Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}