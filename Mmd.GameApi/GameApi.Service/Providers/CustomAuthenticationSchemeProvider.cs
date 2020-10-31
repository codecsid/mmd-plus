using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GameApi.Service.Providers
{
    public class CustomAuthenticationSchemeProvider : AuthenticationSchemeProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CustomAuthenticationSchemeProvider(IHttpContextAccessor httpContextAccessor, IOptions<AuthenticationOptions> options)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException("HttpContextAccessor");   
        }

        private async Task<AuthenticationScheme> GetRequestSchemeAsync()
        {
            var request = httpContextAccessor.HttpContext?.Request;
            if(request == null)
            {
                throw new ArgumentNullException("The Http request cannot be retreived");
            }

            return await GetSchemeAsync("BasicAuth");
        }

        public override async Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync() => await GetRequestSchemeAsync() ?? await base.GetDefaultAuthenticateSchemeAsync();
    }
}