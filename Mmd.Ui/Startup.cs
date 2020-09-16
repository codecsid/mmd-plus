using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Data.Abstract;
using DataAccess.Data.Repositories;
using DataAccess.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService;
using SendGrid;

namespace mmd_plus
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLogging();

            services.AddScoped(typeof(IEntityBaseRepository<>), typeof(EntityBaseRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IRegistrationService, RegistrationService>();

            services.AddScoped<EmailNotificationService>();

            services.AddScoped<Func<string, INotificationService>>(ServiceProvider => key =>
            {
                switch (key)
                {
                    case "Email":
                        return ServiceProvider.GetService<EmailNotificationService>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");

            if (!string.IsNullOrWhiteSpace(smtpHost))
            {
                var fromAddress = new MailAddress(Environment.GetEnvironmentVariable("FROM_ADDRESS"), "Meghraj@CodeComp");
                var fromPassword = Environment.GetEnvironmentVariable("FROM_PASSWORD");
                services.AddSingleton(factory =>
                {
                    return new SmtpClient(smtpHost)
                    {
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                });
            }

            var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            services.AddSingleton(factory => {
                return new SendGridClient(sendGridApiKey);
            });

            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? Configuration.GetConnectionString("CodeCompDatabase");

            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddMvc(options => 
            {
                options.EnableEndpointRouting = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
