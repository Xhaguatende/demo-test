// ----------------------------------------------------------------------------
//  <copyright file="ServiceToService.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class ServiceToService
{
    public string ClaimName { get; set; } = default!;
    public string AppTokenHeaderName { get; set; } = default!;
    public string AppToken { get; set; } = default!;
}
