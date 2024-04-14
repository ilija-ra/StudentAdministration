using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using StudentAdministration.Api.Identity;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Fabric;
using System.Text;

namespace StudentAdministration.Api
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Api : StatelessService
    {
        public Api(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);

                        builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
                        builder.Services.AddScoped<JwtManager>();
                        builder.Services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(o => {
                            o.RequireHttpsMetadata = false;
                            o.SaveToken = false;
                            o.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero,
                                ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
                                ValidAudience = builder.Configuration["JWTSettings:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]!))
                            };
                            o.Events = new JwtBearerEvents()
                            {
                                OnAuthenticationFailed = c =>
                                {
                                    c.NoResult();
                                    c.Response.StatusCode = 401;
                                    c.Response.ContentType = "text/plain";
                                    if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                        c.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");

                                    return c.Response.WriteAsync(c.Exception.ToString());
                                },
                                OnChallenge = context =>
                                {
                                    context.HandleResponse();
                                    context.Response.StatusCode = 401;
                                    context.Response.ContentType = "application/json";
                                    var result = "Not Authorized";
                                    return context.Response.WriteAsync(result);
                                },
                                OnForbidden = context =>
                                {
                                    context.Response.StatusCode = 403;
                                    context.Response.ContentType = "application/json";
                                    var result = "Forbidden action for this role";
                                    return context.Response.WriteAsync(result);
                                }
                            };
                        });

                        builder.Services.AddAuthorization(options =>
                        {
                            options.AddPolicy(IdentityData.RequireProfessorRole, policy =>
                                policy.RequireClaim(CustomClaimTypes.Role, IdentityData.ProfessorRole));

                            options.AddPolicy(IdentityData.RequireStudentRole, policy =>
                                policy.RequireClaim(CustomClaimTypes.Role, IdentityData.StudentRole));
                        });

                        builder.Services.AddControllers();

                        //builder.Services.AddScoped<IValidator<AccountLoginRequestModel>, AccountLoginRequestModelValidator>();
                        //builder.Services.AddScoped<IValidator<AccountRegisterRequestModel>, AccountRegisterRequestModelValidator>();
                        //builder.Services.AddScoped<IValidator<SubjectDropOutRequestModel>, SubjectDropOutRequestModelValidator>();
                        //builder.Services.AddScoped<IValidator<SubjectEnrollRequestModel>, SubjectEnrollRequestModelValidator>();
                        //builder.Services.AddScoped<IValidator<SubjectSetGradesRequestModel>, SubjectSetGradesRequestModelValidator>();
                        //builder.Services.AddScoped<IValidator<UserUpdateRequestModel>, UserUpdateRequestModelValidator>();


                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();
                        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

                        var app = builder.Build();
                        if (app.Environment.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI();
                        }
                        app.UseHttpsRedirection();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.MapControllers();

                        return app;

                    }))
            };
        }
    }
}
