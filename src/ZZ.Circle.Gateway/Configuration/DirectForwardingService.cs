// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingService.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

/// <summary>
/// Direct Forwarding Service
/// </summary>
public class DirectForwardingService
{
    public string AuthenticationScheme { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public List<string> PropagateHeaders { get; set; } = [];
    public bool RemoveHostHeader { get; set; }
    public List<string> RemoveRoutePrefixes { get; set; } = [];
    public List<string> Routes { get; set; } = [];
}
