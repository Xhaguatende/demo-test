// ----------------------------------------------------------------------------
//  <copyright file="GraphQLSettingsValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

// ReSharper disable once UnusedMember.Global
// ReSharper disable once InconsistentNaming
public class GraphQLSettingsValidator : AbstractValidator<GraphQLSettings>
{
    public GraphQLSettingsValidator() => RuleFor(x => x.ExecutionTimeoutMilliseconds).GreaterThan(0);
}
