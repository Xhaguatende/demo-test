// ----------------------------------------------------------------------------
//  <copyright file="JwtSettingsValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class JwtSettingsValidator : AbstractValidator<JwtSettings>
{
    public JwtSettingsValidator()
    {
        RuleFor(x => x.Audience).NotEmpty();
        RuleFor(x => x.Issuer).NotEmpty();
        RuleFor(x => x.Secret).NotEmpty();
    }
}
