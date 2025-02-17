// ----------------------------------------------------------------------------
//  <copyright file="BasicAuthenticationValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class BasicAuthenticationValidator : AbstractValidator<BasicAuthentication>
{
    public BasicAuthenticationValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
