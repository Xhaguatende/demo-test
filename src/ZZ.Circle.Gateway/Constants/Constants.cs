// ----------------------------------------------------------------------------
//  <copyright file="Constants.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Constants;

// ReSharper disable once InconsistentNaming
public static class Constants
{
    public const string AuthorizationHeaderName = "Authorization";
    public const string AzureAdB2CConfigurationName = "AzureAdB2C";
    public const string AzureAdB2CSchemaName = "AzureAdB2C";
    public const string BasicAuthenticationConfigurationName = "BasicAuthentication";
    public const string BasicAuthenticationSchemaName = "BasicAuthentication";
    public const string DirectForwardingConfigurationName = "DirectForwarding";
    public const string DirectForwardingFeaturesConfigurationName = "DirectForwardingFeatures";
    public const string ForwardingConfigurationName = "Forwarding";
    public const string GraphQlAuthenticationPolicyName = "GraphQLGateway";
    public const string GraphQLConfigurationName = "GraphQLSettings";
    public const string GraphQlServersConfigurationName = "GraphQLServers";
    public const string InvalidAuthenticationHeaderMessage = "Invalid Authorization Header";
    public const string JwtAuthenticationSchemaName = "JwtCircleAuthentication";
    public const string JwtConfigurationName = "JwtSettings";
    public const string ObservabilityConfigurationName = "Observability";
    public const string RedisConfigurationName = "RedisSettings";
    public const string ServiceToServiceConfigurationName = "ServiceToService";
    public const string WwwAuthenticateKey = "WWW-Authenticate";
    public const string WwwAuthenticateValue = "Basic realm=\"GraphQL requests authentication details\"";
}
