// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingServiceValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class DirectForwardingServiceValidator : AbstractValidator<DirectForwardingService>
{
    public DirectForwardingServiceValidator()
    {
        RuleFor(x => x.Destination).NotEmpty();

        RuleFor(x => x.Routes).NotEmpty();
    }
}
