using System.Text.RegularExpressions;
using App.Backend.Dto;
using App.Backend.Json;
using App.Backend.Mvc;
using App.Backend.OpenApi;
using App.Lib.Configuration;
using App.Lib.Configuration.Options;
using App.Lib.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace App.Backend;

public class Startup
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
            options.ModelBinderProviders.Insert(0, new JsonBinderProvider());
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new RangeParameterJsonConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.ParameterFilter<RangeFilter>();
            config.OperationFilter<RangeFilter>();
            config.SchemaFilter<NonNullableFilter>();
            config.TagActionsBy(api =>
            {
                return new[] { GetControllerTag(api.ActionDescriptor) };
            });
        });
        services.AddProblemDetails();
        services.AddResponseCompression();

        var corsPolicy = new CorsPolicyBuilder();
        services.PostConfigure<AppOptions>(appOptions =>
        {
            corsPolicy
                .WithOrigins(appOptions.FrontendUrl.TrimEnd('/'))
                .WithExposedHeaders("Content-Range")
                .WithMethods("GET", "POST", "DELETE", "PUT")
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

    private static string GetControllerTag(ActionDescriptor descriptor)
    {
        var controllerName = descriptor.RouteValues["controller"]
            ?? throw new Exception("Can not determine controller name from route values");
        var matches = Regex.Matches(controllerName, "[A-Z][a-z]*");
        if (matches.Count <= 1)
        {
            throw new Exception("Can not determine controller name from route values");
        }

        var lastMatchIndex = matches[^1].Index;
        return controllerName[..lastMatchIndex];
    }
}