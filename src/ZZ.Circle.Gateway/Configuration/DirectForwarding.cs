// ----------------------------------------------------------------------------
//  <copyright file="DirectForwarding.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class DirectForwarding
{
    public List<DirectForwardingService> Services { get; set; } = [];
}
