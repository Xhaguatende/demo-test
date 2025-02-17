// ----------------------------------------------------------------------------
//  <copyright file="GraphQLServer.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

// ReSharper disable once InconsistentNaming
public class GraphQLServer
{
    public bool AddAsRemoteSchema { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
}