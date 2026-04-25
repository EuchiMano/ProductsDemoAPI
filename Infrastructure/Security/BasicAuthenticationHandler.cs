using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using NewProjectFromScratch.Application.Interfaces;

namespace NewProjectFromScratch.Infrastructure.Security
{
    public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authorizationHeader))
                {
                    return AuthenticateResult.NoResult();
                }

                var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
                if (!string.Equals(authHeader.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    return AuthenticateResult.Fail("Invalid authorization header.");
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid basic authentication credentials.");
                }

                var username = credentials[0];
                var password = credentials[1];
                var user = await _userService.ValidateCredentialsAsync(username, password);

                if (user is null)
                {
                    return AuthenticateResult.Fail("Invalid username or password.");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid authorization header.");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"NewProjectFromScratch\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}
