// ----------------------------------------------------------------------------
//  <copyright file="Forwarding.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class Forwarding
{
    public List<string> HeadersToPropagateDownstream { get; set; } = new();
}
