using System.Security.Claims;
using System.Text;
using System.Net.Http.Headers;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccess.Data.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameApi.Service.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ISecurityService _securityService;
        private ILogger _logger;
        private IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration,
            ISecurityService securityService
        ) : base(options, logger, encoder, clock)
        {
            _securityService = securityService ?? throw new ArgumentNullException("securityService");
            _configuration = configuration ?? throw new ArgumentNullException("configuration");
            _logger = logger.CreateLogger("logger") ?? throw new ArgumentNullException("logger");
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");
            
            bool auth = false;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                if(!String.IsNullOrWhiteSpace(username))
                    username = username.ToLowerInvariant();
                var secretToken = credentials[1];

                auth = await _securityService.AuthenticateTeam(username, secretToken);

                if(auth)
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.Name, username)
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    _logger.LogWarning($"Authentication Failure for {username}");
                    return AuthenticateResult.Fail("Invalid Username or SecretToken");
                }
            }
            catch
            {
                _logger.LogError("Authentication Failure");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}