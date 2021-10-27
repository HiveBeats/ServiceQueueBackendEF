#define MIGRATE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Configuration;
using Microsoft.AspNetCore.Identity;
using WebApi.Middlewares;

namespace WebApi
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
            services.AddControllers();
            var connectionString = Configuration.GetConnectionString("QueueDb");
            services.AddDbContext<AppDbContext>(options => options
                .UseMySql(connectionString)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                );

            services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>();

            #if !MIGRATE

            var jwtConfigSection = Configuration.GetSection("jwtTokenConfig");
            var jwtConfig = jwtConfigSection.Get<JwtTokenConfig>();
            services.Configure<JwtTokenConfig>(jwtConfigSection);
            services.AddJwt(jwtConfig);

            services.AddHttpContextAccessor();

            InjectAllFeatures(services);
            
            #endif

            services.AddHealthChecks();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Backend",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "E1Lama",
                        Url = new Uri("https://e1lama.ru")
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                {
                    In = ParameterLocation.Header, 
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey 
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    { 
                        new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" 
                            } 
                        },
                        new string[] { } 
                    } 
                });
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else 
            {
                //migrate in production
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                }
            }

            app.UseHttpsRedirection();
            app.UseHealthChecks("/health_check");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            });
            
            app.UseRouting();
            /* 
            Serving Frontend Feature

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                HttpsCompression = HttpsCompressionMode.Compress,
                OnPrepareResponse = (ctx) =>
                {
                    if (!env.IsDevelopment())
                    {
                        if (ctx.File.Name == "index.html")
                        {
                            ctx.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                            ctx.Context.Response.Headers.Add("Expires", "-1");
                        }
                        else
                        {
                            ctx.Context.Response.Headers.Add("Cache-Control", "max-age=31536000");
                            ctx.Context.Response.Headers.Add("Expires", "31536000");
                        }
                    }
                }
            });
            */
            /*
            
            // Отключаем проверку SSL сертификатов
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            
            */
            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void InjectAllFeatures(IServiceCollection services)
        {
            var featureInjectors = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from assemblyType in domainAssembly.GetExportedTypes()
                                    where assemblyType.IsSubclassOf(typeof(InjectorBase))
                                    select assemblyType).ToArray();
            
            foreach (var i in featureInjectors)
            {
                var instance = Activator.CreateInstance(i);
                (instance as InjectorBase)?.Inject(services);
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddJwt(this IServiceCollection services, JwtTokenConfig jwtConfig)
        {
            services.AddSingleton(jwtConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                //Разрешим авторизацию без https
                //x.RequireHttpsMetadata = false;
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
                    ValidAudience = jwtConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        }
    }
}