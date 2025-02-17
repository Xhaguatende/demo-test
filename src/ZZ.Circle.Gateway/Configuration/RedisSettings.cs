// ----------------------------------------------------------------------------
//  <copyright file="RedisSettings.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class RedisSettings
{
    public string RedisConnectionString { get; set; } = string.Empty;

    public string HotChocolateNamespace { get; set; } = string.Empty;
}