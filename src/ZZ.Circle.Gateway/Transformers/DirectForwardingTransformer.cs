// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingTransformer.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Transformers;

using Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

public class DirectForwardingTransformer : HttpTransformer
{
    private readonly List<DirectForwardingService> _services;

    public DirectForwardingTransformer(List<DirectForwardingService> services)
    {
        _services = services;
    }

    public override async ValueTask TransformRequestAsync(
        HttpContext httpContext,
        HttpRequestMessage proxyRequest,
        string destinationPrefix,
        CancellationToken cancellationToken)
    {
        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

        var route = httpContext.GetEndpoint();

        var service = _services.First(x => x.Routes.Contains(route?.DisplayName ?? string.Empty));

        CheckHeaders(proxyRequest, service);

        var destination = GetDestination(httpContext, service);

        var queryContext = new QueryTransformContext(httpContext.Request);

        proxyRequest.RequestUri = RequestUtilities.MakeDestinationAddress(
            destination,
            string.Empty,
            queryContext.QueryString);
    }

    private static void CheckHeaders(HttpRequestMessage proxyRequest, DirectForwardingService service)
    {
        proxyRequest.Headers.Host = service.RemoveHostHeader ? null : proxyRequest.Headers.Host;

        var headersToPropagate = proxyRequest.Headers
            .Where(h => service.PropagateHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Value);

        foreach (var kv in headersToPropagate)
        {
            proxyRequest.Headers.Add(kv.Key, kv.Value);
        }
    }

    private static string GetDestination(HttpContext httpContext, DirectForwardingService service)
    {
        var path = httpContext.Request.Path.Value ?? string.Empty;

        if (service.RemoveRoutePrefixes.Count != 0)
        {
            foreach (var prefix in service.RemoveRoutePrefixes)
            {
                path = path.Replace(prefix, string.Empty);
            }
        }

        var destination = string.Format(service.Destination, path);

        return destination;
    }
}
