// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingFeaturesValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class DirectForwardingFeaturesValidator : AbstractValidator<DirectForwardingFeatures>
{
    public DirectForwardingFeaturesValidator()
    {
        RuleFor(x => x.OutboundActivityTimeoutInSeconds).GreaterThan(0);

        RuleFor(x => x.OutboundConnectTimeoutInSeconds).GreaterThanOrEqualTo(0);
    }
}
