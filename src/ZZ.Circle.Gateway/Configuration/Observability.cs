// ----------------------------------------------------------------------------
//  <copyright file="Observability.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class Observability
{
    public string AzureAppInsightsConnectionString { get; set; } = string.Empty;
    public List<string> IgnoreRequestsForTelemetryMatching { get; set; } = new();
    public string OtlpEndpoint { get; set; } = string.Empty;
    public string OtlpHeaders { get; set; } = string.Empty;
    public string? ServiceName { get; set; }
}