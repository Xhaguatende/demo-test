// ----------------------------------------------------------------------------
//  <copyright file="BuilderExtensions.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using System.Reflection;
using System.Runtime.InteropServices;
using Azure.Monitor.OpenTelemetry.Exporter;
using Configuration;
using Handlers;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Constants = Constants.Constants;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        // Configure OpenTelemetry service resource details
        var entryAssembly = Assembly.GetEntryAssembly();
        var entryAssemblyName = entryAssembly?.GetName();
        var versionAttribute = entryAssembly?.GetCustomAttributes(false)
            .OfType<AssemblyInformationalVersionAttribute>()
            .FirstOrDefault();

        var attributes = new Dictionary<string, object>
        {
            // See https://github.com/open-telemetry/opentelemetry-specification/tree/main/specification/resource/semantic_conventions
            ["host.name"] = Environment.MachineName,
            ["os.description"] = RuntimeInformation.OSDescription,
            ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant()
        };

        var config = builder.Configuration.GetSection(Constants.ObservabilityConfigurationName).Get<Observability>();

        var serviceName = config!.ServiceName ?? entryAssemblyName?.Name ?? "UnknownServiceName";
        var serviceVersion = versionAttribute?.InformationalVersion ??
                             entryAssemblyName?.Version?.ToString() ?? "UnknownServiceVersion";

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: serviceVersion)
            .AddTelemetrySdk()
            .AddAttributes(attributes);

        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            "development",
            StringComparison.InvariantCultureIgnoreCase);

        builder.Logging.ClearProviders()
            .AddOpenTelemetry(
                options =>
                {
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.SetResourceBuilder(resourceBuilder);

                    VerifyConsoleExporterLogging(isDevelopment, options);

                    VerifyOtlpLogging(config, options);

                    VerifyAzureAppInsightsLogging(config, options);
                })
            .AddConfiguration(builder.Configuration);

        builder.Services.AddOpenTelemetry()
            .WithTracing(
                b =>
                {
                    b.AddProcessor<TelemetryFilteringProcessor>();
                    b.SetResourceBuilder(resourceBuilder);
                    b.AddHttpClientInstrumentation();
                    b.AddHotChocolateInstrumentation();
                    b.AddAspNetCoreInstrumentation(opt => { opt.RecordException = true; });

                    VerifyConsoleExporterTracing(isDevelopment, b);
                    VerifyZipkinExporterTracing(isDevelopment, b);
                    VerifyOtlpTracing(config, b);
                    VerifyAzureAppInsightsTracing(config, b);
                });
        return builder;
    }

    public static WebApplicationBuilder ConfigureGraphQlServers(this WebApplicationBuilder builder)
    {
        var graphQlServers = builder.Configuration.GetSection(Constants.GraphQlServersConfigurationName)
            .Get<List<GraphQLServer>>();

        graphQlServers!.ForEach(
            x =>
            {
                var route = new Uri(x.Endpoint);
                builder.Services
                    .AddHttpClient(x.SchemaName, c => c.BaseAddress = route)
                    .AddHttpMessageHandler<CancellationPropagationHandler>()
                    .AddHeaderPropagation();
            });

        return builder;
    }

    private static void VerifyAzureAppInsightsLogging(Observability config, OpenTelemetryLoggerOptions options)
    {
        if (!string.IsNullOrEmpty(config.AzureAppInsightsConnectionString))
        {
            options.AddAzureMonitorLogExporter(
                o => { o.ConnectionString = config.AzureAppInsightsConnectionString; });
        }
    }

    private static void VerifyAzureAppInsightsTracing(Observability config, TracerProviderBuilder builder)
    {
        if (!string.IsNullOrEmpty(config.AzureAppInsightsConnectionString))
        {
            builder.AddAzureMonitorTraceExporter(
                options => { options.ConnectionString = config.AzureAppInsightsConnectionString; });
        }
    }

    private static void VerifyConsoleExporterLogging(bool isDevelopment, OpenTelemetryLoggerOptions options)
    {
        if (isDevelopment)
        {
            options.AddConsoleExporter();
        }
    }

    private static void VerifyConsoleExporterTracing(bool isDevelopment, TracerProviderBuilder builder)
    {
        if (isDevelopment)
        {
            builder.AddConsoleExporter();
        }
    }

    private static void VerifyOtlpLogging(Observability config, OpenTelemetryLoggerOptions options)
    {
        if (!string.IsNullOrEmpty(config.OtlpEndpoint))
        {
            options.AddOtlpExporter(
                o =>
                {
                    o.Endpoint = new Uri(config.OtlpEndpoint);
                    if (!string.IsNullOrEmpty(config.OtlpHeaders))
                    {
                        o.Headers = config.OtlpHeaders;
                    }
                });
        }
    }

    private static void VerifyOtlpTracing(Observability config, TracerProviderBuilder builder)
    {
        if (!string.IsNullOrEmpty(config.OtlpEndpoint))
        {
            builder.AddOtlpExporter(
                o =>
                {
                    o.Endpoint = new Uri(config.OtlpEndpoint);
                    if (!string.IsNullOrEmpty(config.OtlpHeaders))
                    {
                        o.Headers = config.OtlpHeaders;
                    }
                });
        }
    }

    private static void VerifyZipkinExporterTracing(bool isDevelopment, TracerProviderBuilder builder)
    {
        if (isDevelopment)
        {
            builder.AddZipkinExporter();
        }
    }
}
