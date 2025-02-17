// ----------------------------------------------------------------------------
//  <copyright file="DirectForwardingValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class DirectForwardingValidator : AbstractValidator<DirectForwarding>
{
    public DirectForwardingValidator()
    {
        RuleForEach(x => x.Services)
            .SetValidator(new DirectForwardingServiceValidator());

        RuleFor(x => x.Services)
            .Must(
                x =>
                {
                    var routes = x.SelectMany(s => s.Routes).ToList();
                    var hasDuplicates = routes.Count != routes.Distinct(StringComparer.OrdinalIgnoreCase).Count();

                    return !hasDuplicates;
                })
            .WithMessage("Cannot have duplicate routes!");
    }
}
