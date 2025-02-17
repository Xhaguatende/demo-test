// ----------------------------------------------------------------------------
//  <copyright file="AzureAdB2C.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Configuration;

public class AzureAdB2C
{
    public string Instance { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public string SignUpSignInPolicyId { get; set; } = string.Empty;
}