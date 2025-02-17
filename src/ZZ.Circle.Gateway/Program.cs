using System.Diagnostics;
using FluentValidation;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using ZZ.Circle.Gateway.Configuration;
using ZZ.Circle.Gateway.Constants;
using ZZ.Circle.Gateway.Extensions;
using ZZ.Circle.Gateway.Formatters;
using ZZ.Circle.Gateway.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureHost();

var services = builder.Services;

services.AddHttpContextAccessor();
services.AddTransient<CancellationPropagationHandler>();
services.AddHttpResponseFormatter<ResponseCodeSerializer>();
services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

services.AddHeaderPropagation(options =>
{
    builder.Configuration.GetSection(Constants.ForwardingConfigurationName).Get<Forwarding>()!
        .HeadersToPropagateDownstream.ForEach(x => options.Headers.Add(x));
});

services.ConfigureOptions(builder.Configuration);

builder.AddOpenTelemetry();

services.ConfigureAuthentication();

services.ConfigureAuthorization();

services.AddCors();

builder.ConfigureGraphQlServers();

var redisSettings = builder.Configuration.GetSection(Constants.RedisConfigurationName).Get<RedisSettings>();

var redisConnectionString = redisSettings!.RedisConnectionString;
var hotChocolateNamespace = redisSettings.HotChocolateNamespace;

var graphQlSettings = builder.Configuration.GetSection(Constants.GraphQLConfigurationName).Get<GraphQLSettings>();

builder.Services
    .AddSingleton(await ConnectionMultiplexer.ConnectAsync(redisConnectionString))
    .AddGraphQLServer()
    .ModifyRequestOptions(o => o.ExecutionTimeout = TimeSpan.FromMilliseconds(graphQlSettings?.ExecutionTimeoutMilliseconds ?? 30000))
    .AddInstrumentation(o =>
    {
        o.RequestDetails = HotChocolate.Diagnostics.RequestDetails.All;
        o.Scopes = HotChocolate.Diagnostics.ActivityScopes.ExecuteHttpRequest;
    })
    .AddRemoteSchemasFromRedis(hotChocolateNamespace, serviceProvider => serviceProvider.GetRequiredService<ConnectionMultiplexer>())
    .AddRemoteSchemasFromConfiguration(builder.Configuration);

services.AddHttpForwarder();

services.AddAuthorizationBuilder()
    .AddPolicy(
        Constants.JwtAuthenticationSchemaName,
        policy =>
        {
            policy.AddAuthenticationSchemes(Constants.JwtAuthenticationSchemaName);
            policy.RequireAuthenticatedUser().Build();
        })
    .AddPolicy(Constants.BasicAuthenticationSchemaName,
        policy =>
        {
            policy.AddAuthenticationSchemes(Constants.BasicAuthenticationSchemaName);
            policy.RequireAuthenticatedUser().Build();
        })
    .AddPolicy(Constants.AzureAdB2CSchemaName,
        policy =>
        {
            policy.AddAuthenticationSchemes(Constants.AzureAdB2CSchemaName);
            policy.RequireAuthenticatedUser().Build();
        });

var app = builder.Build();
app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHeaderPropagation();

app.Use(async (context, next) =>
{
    var serviceToServiceConfig = context.RequestServices.GetRequiredService<IOptions<ServiceToService>>().Value;

    if (Activity.Current != null)
    {
        context.Response.Headers.Append("X-Trace-Id", Activity.Current.Context.TraceId.ToString());
    }

    if (!string.IsNullOrEmpty(serviceToServiceConfig.ClaimName))
    {
        var isSystemToSystemUser = context.User.Claims.Any(c => c.Type == serviceToServiceConfig.ClaimName);
        var isAppTokenCorrect = context.Request.Headers[serviceToServiceConfig.AppTokenHeaderName] == serviceToServiceConfig.AppToken;
        if (isSystemToSystemUser && !isAppTokenCorrect)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden!");
            return;
        }
    }

    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL().RequireAuthorization(Constants.GraphQlAuthenticationPolicyName);

app.UseDirectForwarding();

await app.RunAsync();
