﻿using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Postcore.Web.Attributes;
using Postcore.Web.Core.Interfaces;
using Postcore.Web.Helpers;
using Postcore.Web.Hubs;
using Postcore.Web.Infrastructure.ApiClients;
using Postcore.Web.Infrastructure.Mapper;
using Postcore.Web.Infrastructure.Utilities;
using System;
using System.Net;
using System.Net.Http;

namespace Postcore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AWSXRayRecorder.InitializeInstance(Configuration);
            AWSSDKHandler.RegisterXRayForAllServices();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCognitoIdentity(config =>
            {
                config.Password = new Microsoft.AspNetCore.Identity.PasswordOptions
                {
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequiredUniqueChars = 0,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IMapper, Mapper>();
            services.AddTransient<IFileUploader, S3FileUploader>();

            services.AddTransient<HttpClientXRayTracingHandler>();
            services.AddHttpClient<IAdApiClient, AdApiClient>()
                .AddHttpMessageHandler<HttpClientXRayTracingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPatternPolicy());

            services.AddHttpClient<ISearchApiClient, SearchApiClient>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPatternPolicy());

            services.Configure<GoogleAnalyticsSettings>(settings => Configuration.GetSection("GoogleAnalytics").Bind(settings));
            services.AddTransient<ITagHelperComponent, GoogleAnalyticsTagHelperComponent>();

            services.AddTransient<ISnsAdapter, SnsAdapter>();

            services.AddMvc(options => 
            {
                options.Filters.Add(new RequireWwwAttribute
                {
                    IgnoreLocalhost = true, Permanent = true
                });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            });
        }

        // 5 retry attemps
        // 2^exp where exp = 1 -> 5
        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPatternPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseXRay("Postcore.Web");
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SnsHub>("/snsHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
