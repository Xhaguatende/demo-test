// ----------------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using System.Text;
using Authentication;
using Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Validators;
using Constants = Constants.Constants;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var adB2CConfiguration = serviceProvider.GetRequiredService<IOptions<AzureAdB2C>>().Value;

        if (!(string.IsNullOrEmpty(adB2CConfiguration.Instance) ||
              string.IsNullOrEmpty(adB2CConfiguration.Domain) ||
              string.IsNullOrEmpty(adB2CConfiguration.ClientId) ||
              string.IsNullOrEmpty(adB2CConfiguration.SignUpSignInPolicyId)))
        {
            services.AddAuthentication(Constants.AzureAdB2CSchemaName)
                .AddMicrosoftIdentityWebApi(
                    _ =>
                    {
                    },
                    options =>
                    {
                        options.Instance = adB2CConfiguration.Instance;
                        options.ClientId = adB2CConfiguration.ClientId;
                        options.Domain = adB2CConfiguration.Domain;
                        options.SignUpSignInPolicyId = adB2CConfiguration.SignUpSignInPolicyId;
                    },
                    Constants.AzureAdB2CSchemaName);
        }

        var basicAuthenticationConfiguration =
            serviceProvider.GetRequiredService<IOptions<BasicAuthentication>>().Value;

        if (!(string.IsNullOrEmpty(basicAuthenticationConfiguration.Username) ||
              string.IsNullOrEmpty(basicAuthenticationConfiguration.Password)))
        {
            services.AddAuthentication(Constants.AzureAdB2CSchemaName)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                    Constants.BasicAuthenticationSchemaName,
                    null);
        }

        var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                Constants.JwtAuthenticationSchemaName,
                options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

        return services;
    }

    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        // The authorization policies are defined as:
        //  - All the authentication schemes: used by the GraphQL Gateway
        //  - Single authentication scheme: used by the Direct Forwarding (if configured).

        services.AddAuthorizationBuilder()
            .AddPolicy(
                Constants.GraphQlAuthenticationPolicyName,
                policy =>
                {
                    policy.AddAuthenticationSchemes(
                        Constants.AzureAdB2CSchemaName,
                        Constants.BasicAuthenticationSchemaName,
                        Constants.JwtAuthenticationSchemaName);

                    policy.RequireAuthenticatedUser().Build();
                })
            .AddPolicy(
                Constants.JwtAuthenticationSchemaName,
                policy =>
                {
                    policy.AddAuthenticationSchemes(Constants.JwtAuthenticationSchemaName);
                    policy.RequireAuthenticatedUser().Build();
                })
            .AddPolicy(
                Constants.BasicAuthenticationSchemaName,
                policy =>
                {
                    policy.AddAuthenticationSchemes(Constants.BasicAuthenticationSchemaName);
                    policy.RequireAuthenticatedUser().Build();
                })
            .AddPolicy(
                Constants.AzureAdB2CSchemaName,
                policy =>
                {
                    policy.AddAuthenticationSchemes(Constants.AzureAdB2CSchemaName);
                    policy.RequireAuthenticatedUser().Build();
                });

        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOptions<Forwarding>()
            .Bind(configuration.GetSection(Constants.ForwardingConfigurationName));

        services.AddOptions<Observability>()
            .Bind(configuration.GetSection(Constants.ObservabilityConfigurationName));

        services.AddOptions<BasicAuthentication>()
            .Bind(configuration.GetSection(Constants.BasicAuthenticationConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<AzureAdB2C>()
            .Bind(configuration.GetSection(Constants.AzureAdB2CConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<ServiceToService>()
            .Bind(configuration.GetSection(Constants.ServiceToServiceConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(Constants.JwtConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<GraphQLServer>()
            .Bind(configuration.GetSection(Constants.GraphQlServersConfigurationName));

        services.AddOptions<RedisSettings>()
            .Bind(configuration.GetSection(Constants.RedisConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<GraphQLSettings>()
            .Bind(configuration.GetSection(Constants.GraphQLConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<DirectForwarding>()
            .Bind(configuration.GetSection(Constants.DirectForwardingConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        services.AddOptions<DirectForwardingFeatures>()
            .Bind(configuration.GetSection(Constants.DirectForwardingFeaturesConfigurationName))
            .ValidateFluently()
            .ValidateOnStart();

        return services;
    }
}
