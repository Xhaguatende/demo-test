// ----------------------------------------------------------------------------
//  <copyright file="WebApplicationExtensions.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using System.Diagnostics;
using System.Net;
using Configuration;
using Microsoft.Extensions.Options;
using Transformers;
using Yarp.ReverseProxy.Forwarder;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseDirectForwarding(this WebApplication app)
    {
        var forwardingFeatures = app.Services.GetRequiredService<IOptions<DirectForwardingFeatures>>().Value;

        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = forwardingFeatures.OutboundConnectTimeoutInSeconds != 0 ?
                TimeSpan.FromSeconds(forwardingFeatures.OutboundConnectTimeoutInSeconds) :
                Timeout.InfiniteTimeSpan
        });

        var requestConfig = new ForwarderRequestConfig
        {
            ActivityTimeout = TimeSpan.FromSeconds(forwardingFeatures.OutboundActivityTimeoutInSeconds)
        };

        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var directForwardingServices = serviceScope.ServiceProvider.GetRequiredService<IOptions<DirectForwarding>>().Value.Services;

        var transformer = new DirectForwardingTransformer(directForwardingServices);

        foreach (var service in directForwardingServices)
        {
            foreach (var serviceRoute in service.Routes)
            {
                var routeBuilder = app.Map(serviceRoute, async (HttpContext httpContext, IHttpForwarder forwarder) =>
                {
                    var error = await forwarder.SendAsync(httpContext, service.Destination, httpClient, requestConfig, transformer);

                    CheckForwarderError(error, httpContext);
                });

                if (!string.IsNullOrWhiteSpace(service.AuthenticationScheme))
                {
                    routeBuilder.RequireAuthorization(service.AuthenticationScheme);
                }
            }
        }

        return app;
    }

    private static void CheckForwarderError(ForwarderError error, HttpContext httpContext)
    {
        if (error == ForwarderError.None)
        {
            return;
        }

        var errorFeature = httpContext.GetForwarderErrorFeature();
        var exception = errorFeature?.Exception;

        var activity = Activity.Current;

        if (exception is not null)
        {
            activity?.AddException(exception);
        }
    }
}
