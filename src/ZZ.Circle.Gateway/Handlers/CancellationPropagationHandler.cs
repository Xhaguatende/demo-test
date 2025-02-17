// ----------------------------------------------------------------------------
//  <copyright file="CancellationPropagationHandler.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Handlers;

public class CancellationPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CancellationPropagationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            httpContext.RequestAborted,
            cancellationToken);

        return await base.SendAsync(request, linkedCts.Token);

    }
}