// ----------------------------------------------------------------------------
//  <copyright file="AzureAdB2CValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class AzureAdB2CValidator : AbstractValidator<AzureAdB2C>
{
    public AzureAdB2CValidator() =>
        RuleFor(x => new[] { x.Instance, x.ClientId, x.Domain, x.SignUpSignInPolicyId })
            .Custom(
                (values, context) =>
                {
                    if (values.Any(x => !string.IsNullOrEmpty(x)) && values.Any(string.IsNullOrEmpty))
                    {
                        context.AddFailure(
                            "All properties must be null/empty, or all properties must not be null/empty.");
                    }
                });
}
