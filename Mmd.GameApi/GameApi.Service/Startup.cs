using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using GameApi.Service.Handlers;
using GameApi.Service.Middleware;
using GameApi.Service.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace GameApi.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuth", null);
            services.AddScoped<IAuthenticationSchemeProvider, CustomAuthenticationSchemeProvider>();
            
            services.AddHttpContextAccessor();
            services.AddLogging(cfg => cfg.AddConsole()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Warning);
            
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IGameApiService, GameApiService>();
            services.AddScoped<IRequestLoggingService, RequestLoggingService>();
            
            services.AddScoped<RequestContext>();
            services.AddScoped<EvaluationModule>();
            
            services.AddScoped<GameContext>(factory =>
            {
                try
                {
                    var context = factory.GetRequiredService<DataContext>();
                    var currentState = context.Phases
                    .Include(p => p.Round)
                        .ThenInclude(p => p.Participants)
                    .OrderByDescending(p => p.TimeStamp).FirstOrDefault();

                    return new GameContext
                    {
                        GameId = currentState.GameId,
                        RoundId = currentState.RoundId,
                        RoundNumber = currentState.Round.RoundNumber,
                        CurrentPhase = currentState.PhaseType,
                        Participants = currentState.Round.Participants.ToList()
                    };
                }
                catch (Exception ex)
                {
                    var logger = factory.GetRequiredService<ILogger<Startup>>();
                    logger.LogError($"GameContext generation failed, {ex.Message}");
                    return new GameContext { Participants = new List<Participant>() };
                }
            });
            
            services.AddControllers().AddNewtonsoftJson();
            
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestRecordingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ThrottlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
