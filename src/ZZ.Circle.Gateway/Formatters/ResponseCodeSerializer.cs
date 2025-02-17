// ----------------------------------------------------------------------------
//  <copyright file="ResponseCodeSerializer.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Formatters;

using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

public class ResponseCodeSerializer : DefaultHttpResponseFormatter
{
    protected override HttpStatusCode OnDetermineStatusCode(
        IQueryResult result,
        FormatInfo format,
        HttpStatusCode? proposedStatusCode)
    {
        if (result is not IQueryResult queryResult || !(queryResult.Errors?.Count > 0))
        {
            return base.OnDetermineStatusCode(result, format, proposedStatusCode);
        }

        foreach (var error in queryResult.Errors)
        {
            var isAuthError = error.Code == "AUTH_NOT_AUTHORIZED" ||
                              (error.Extensions?.Any(ext => ext.Value?.ToString() == "AUTH_NOT_AUTHORIZED") ?? false);

            if (isAuthError)
            {
                return HttpStatusCode.Forbidden;
            }

            var hasOtherErrors = error.Extensions != null &&
                                 error.Extensions.Any(ext => ext.Key?.ToString() == "errorCode");

            if (hasOtherErrors)
            {
                return HttpStatusCode.BadRequest;
            }
        }

        return base.OnDetermineStatusCode(result, format, proposedStatusCode);
    }
}
