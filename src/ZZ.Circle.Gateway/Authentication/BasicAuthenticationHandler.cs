// ----------------------------------------------------------------------------
//  <copyright file="BasicAuthenticationHandler.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Authentication;

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Configuration;
using Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly BasicAuthentication _basicAuthenticationConfiguration;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<BasicAuthentication> basicAuthenticationConfiguration) : base(options, logger, encoder)
    {
        _basicAuthenticationConfiguration = basicAuthenticationConfiguration.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers[Constants.AuthorizationHeaderName].ToString();
        if (authHeader.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("basic", StringComparison.InvariantCultureIgnoreCase))
        {
            var token = authHeader.Replace("Basic ", string.Empty);
            var credentialsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialsString.Split(':');

            var username = credentials.FirstOrDefault();
            var password = credentials.ElementAtOrDefault(1);

            if (username == _basicAuthenticationConfiguration.Username &&
                password == _basicAuthenticationConfiguration.Password)
            {
                var claims = new[] { new Claim("name", username), new Claim(ClaimTypes.Role, username) };
                var claimsIdentity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return Task.FromResult(
                    AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }

            Response.StatusCode = 401;
            Response.Headers.Append(Constants.WwwAuthenticateKey, Constants.WwwAuthenticateValue);

            return Task.FromResult(AuthenticateResult.Fail(Constants.InvalidAuthenticationHeaderMessage));
        }

        Response.StatusCode = 401;
        Response.Headers.Append(Constants.WwwAuthenticateKey, Constants.WwwAuthenticateValue);

        return Task.FromResult(AuthenticateResult.Fail(Constants.InvalidAuthenticationHeaderMessage));
    }
}