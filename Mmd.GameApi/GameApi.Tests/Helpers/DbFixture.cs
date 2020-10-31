using DataAccess.Data;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model.SharedModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GameApi.Tests.Helpers
{
    public class DbFixture
    {
        public DbFixture()
        {
            var services = new ServiceCollection();
            //serviceCollection
            //    .AddDbContext<SomeContext>(options => options.UseSqlServer("connection string"),
            //        ServiceLifetime.Transient);

            services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, FakeBasicAuthenticationHandler>("BasicAuth", null);
            services.AddScoped<RequestContext>();
            services.AddScoped<Mock<DataContext>>(factory =>
            {
                return new Mock<DataContext>();
            });
            services.AddScoped<IGameApiService, GameApiService>();
            services.AddHttpContextAccessor();
            services.AddLogging(cfg => cfg.AddConsole()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Warning);
            services.AddScoped<IAuthenticationSchemeProvider, FakeCustomAuthenticationSchemeProvider>();


            ServiceProvider = services.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }

        public class FakeCustomAuthenticationSchemeProvider : AuthenticationSchemeProvider
        {
            private readonly IHttpContextAccessor httpContextAccessor;

            public FakeCustomAuthenticationSchemeProvider(IHttpContextAccessor httpContextAccessor, IOptions<AuthenticationOptions> options)
                : base(options)
            {
                this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException("HttpContextAccessor");
            }

            private async Task<AuthenticationScheme> GetRequestSchemeAsync()
            {
                var request = httpContextAccessor.HttpContext?.Request;
                if (request == null)
                {
                    throw new ArgumentNullException("The Http request cannot be retreived");
                }

                return await GetSchemeAsync("BasicAuth");
            }

            public override async Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync() => await GetRequestSchemeAsync() ?? await base.GetDefaultAuthenticateSchemeAsync();
        }

        public class FakeBasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            private ILogger _logger;
            private IConfiguration _configuration;

            public FakeBasicAuthenticationHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock,
                IConfiguration configuration
            ) : base(options, logger, encoder, clock)
            {
                _configuration = configuration ?? throw new ArgumentNullException("configuration");
                _logger = logger.CreateLogger("logger") ?? throw new ArgumentNullException("logger");
            }

            protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Missing Authorization Header");

                bool auth = false;
                try
                {
                    //var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    //var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    //var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var username = "FakeUserName";
                    if (!String.IsNullOrWhiteSpace(username))
                        username = username.ToLowerInvariant();
                    var secretToken = "FakeSecretToken";

                    //auth = await _securityService.AuthenticateTeam(username, secretToken);
                    auth = true;
                    if (auth)
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
}
