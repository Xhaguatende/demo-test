// ----------------------------------------------------------------------------
//  <copyright file="HostBuilderExtensions.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using Azure.Identity;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureHost(this IHostBuilder hostBuilder, string[]? args = null)
    {
        return hostBuilder
            .ConfigureAppConfiguration(
                (hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true);
                    config.AddJsonFile("appsettings.local.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                    var appConfigUrl = Environment.GetEnvironmentVariable("APP_CONFIGURL") ?? string.Empty;
                    var appConfigConnectionString =
                        Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION_STRING") ?? string.Empty;
                    var clientId = Environment.GetEnvironmentVariable("APP_CLIENTID") ?? string.Empty;
                    var configPrefix = Environment.GetEnvironmentVariable("APP_CONFIGPREFIX") ?? string.Empty;
                    var configLabel = Environment.GetEnvironmentVariable("APP_CONFIGLABEL") ?? string.Empty;

                    if (!(string.IsNullOrEmpty(configPrefix) || string.IsNullOrEmpty(configLabel)) &&
                        (!string.IsNullOrEmpty(appConfigConnectionString) ||
                         !(string.IsNullOrEmpty(appConfigUrl) && string.IsNullOrEmpty(clientId))))
                    {
                        config.AddAzureAppConfiguration(
                            options =>
                            {
                                (string.IsNullOrEmpty(appConfigConnectionString)
                                        ? options.Connect(
                                            new Uri(appConfigUrl),
                                            new ManagedIdentityCredential(clientId))
                                        : options.Connect(appConfigConnectionString))

                                    .ConfigureKeyVault(
                                        kv => { kv.SetCredential(new ManagedIdentityCredential(clientId)); })
                                    .Select($"{configPrefix}*", configLabel)
                                    .TrimKeyPrefix(configPrefix);
                            });
                    }
                });
    }
}
