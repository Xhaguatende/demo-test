// ----------------------------------------------------------------------------
//  <copyright file="RedisSettingsValidator.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Validators;

using Configuration;
using FluentValidation;

public class RedisSettingsValidator : AbstractValidator<RedisSettings>
{
    public RedisSettingsValidator()
    {
        RuleFor(x => x.RedisConnectionString).NotEmpty();
        RuleFor(x => x.HotChocolateNamespace).NotEmpty();
    }
}
