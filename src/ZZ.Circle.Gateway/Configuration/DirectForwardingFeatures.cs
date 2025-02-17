// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingFeatures.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

/// <summary>
/// Direct Forwarding Features
/// </summary>
public class DirectForwardingFeatures
{
    public int OutboundActivityTimeoutInSeconds { get; set; } = 100;
    public int OutboundConnectTimeoutInSeconds { get; set; } = 0;
}
