using DataAccess.Data;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model.SharedModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GameApi.Tests.Helpers.DbFixture;

namespace GameApi.Tests.Helpers
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, FakeBasicAuthenticationHandler>("BasicAuth", null);
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationSchemeProvider, FakeCustomAuthenticationSchemeProvider>();
            services.AddLogging(cfg => cfg.AddConsole()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Warning);
            services.AddScoped<IGameApiService, GameApiService>();
            services.AddScoped<IRequestLoggingService, RequestLoggingService>();
            services.AddScoped<RequestContext>();
            services.AddControllers().AddNewtonsoftJson();
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            services.AddScoped<Mock<DataContext>>();
            services.AddScoped<DataContext>(factory =>
            {
                return factory.GetService<Mock<DataContext>>().Object;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
