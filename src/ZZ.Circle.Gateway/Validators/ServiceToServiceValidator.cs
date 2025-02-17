// ----------------------------------------------------------------------------
//  <copyright file="ServiceToServiceValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class ServiceToServiceValidator : AbstractValidator<ServiceToService>
{
    public ServiceToServiceValidator() =>
        RuleFor(x => x)
            .Custom(
                (config, context) =>
                {
                    if (string.IsNullOrEmpty(config.AppTokenHeaderName) != string.IsNullOrEmpty(config.AppToken))
                    {
                        context.AddFailure(
                            "AppTokenHeaderName and AppToken must either both be empty or both be configured.");
                    }

                    if (!string.IsNullOrEmpty(config.ClaimName) &&
                        (string.IsNullOrEmpty(config.AppToken) || string.IsNullOrEmpty(config.AppTokenHeaderName)))
                    {
                        context.AddFailure(
                            "AppTokenHeaderName and AppToken must be configured if ClaimName is not empty.");
                    }
                });
}
