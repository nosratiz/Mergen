using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Mergen.Admin.Api.Helpers;
using Mergen.Admin.Api.Middlewares;
using Mergen.Api.Core.QueryProcessing;
using Mergen.Api.Core.Security;
using Mergen.Api.Core.Security.AuthenticationSystem;
using Mergen.Api.Core.Security.AuthorizationSystem;
using Mergen.Api.Core.TimezoneHelpers;
using Mergen.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Mergen.Admin.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterMergenServices(Configuration, _hostingEnvironment.IsDevelopment());

            services.AddLocalization();
            services.AddJwtAuthentication(Configuration);

            services.AddCors(config =>
            {
                config.AddPolicy("AllowCors", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                });
            });

            services.AddMvc(options =>
            {
                if (!bool.TryParse(Configuration["AuthenticationOptions:Disabled"], out var disableAuthentication) ||
                    !disableAuthentication)
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireClaim(JwtRegisteredClaimNames.Jti)
                        .RequireClaim(ClaimTypes.NameIdentifier)
                        //.AddRequirements(new DbAuthorizationRequirement())
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                }

                options.ModelBinderProviders.Insert(0, new QueryModelBindingProvider());
                options.ModelBinderProviders.Insert(0, new DateTimeOffsetModelBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcLocalization()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new DateTimeConverter(new HttpContextAccessor()));
                    options.SerializerSettings.DateParseHandling = DateParseHandling.None;
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Mergen API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()},
                });

                options.OperationFilter<QueryModelOperationFilter>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                    actionContext.ModelState.ToUnprocessableEntityResult();
            });

            services.AddOptions();

            services.Configure<JwtTokenOptions>(Configuration.GetSection("JwtTokenOptions"));
            services.AddSingleton<JwtTokenGenerator>();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ApplicationEvents.ApplicationStart(app.ApplicationServices, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowCors");
            app.UseRequestLocalization(options =>
            {
                options.AddSupportedCultures("en");
                options.AddSupportedUICultures("en");
                options.DefaultRequestCulture = new RequestCulture("en", "en");
                options.RequestCultureProviders = null;
                /*options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CustomRequestCultureProvider(context => Task.FromResult(new ProviderCultureResult("fa", "fa")))
                };*/
            });

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mergen API V1"); });
            app.UseAuthentication();
            app.UseMiddleware<PrincipalWrapperMiddleware>();
            app.UseAuthorization();
            app.UseMvc();

            AutoMapper.Mapper.Initialize(config => { config.CreateMissingTypeMaps = true; });
        }
    }
}