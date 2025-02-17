// ----------------------------------------------------------------------------
//  <copyright file="RequestExecutorBuilderExtensions.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using Configuration;
using Constants;
using HotChocolate.Execution.Configuration;

public static class RequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddRemoteSchemasFromConfiguration(
        this IRequestExecutorBuilder builder,
        ConfigurationManager config)
    {
        var graphQlServers = config.GetSection(Constants.GraphQlServersConfigurationName).Get<List<GraphQLServer>>();

        if (!graphQlServers!.Exists(x => x.AddAsRemoteSchema))
        {
            builder
                .AddQueryType(schemaDescriptor => { schemaDescriptor.Name("Query"); })
                .AddMutationType(schemaDescriptor => { schemaDescriptor.Name("Mutation"); });
        }

        graphQlServers.Where(x => x.AddAsRemoteSchema)
            .ToList()
            .ForEach(x => { builder.AddRemoteSchema(x.SchemaName); });

        return builder;
    }
}
