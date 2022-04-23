using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Commom;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Startup
    {
        public static Dictionary<string, string> Parametros = new Dictionary<string, string>();

        private readonly IConfiguration _config;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = configuration;
            //API.Config.ConfigDB.Configurar();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors();
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            
            services.Configure<DBConnection>(Configuration.GetSection("ParametrosAcx"));

            Parametros["ConnectionString"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:DefaultConnection");
            Parametros["database"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:database");
            Parametros["host"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:host");
            Parametros["user"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:user");
            Parametros["pass"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:pass");
            Parametros["dbname"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:dbname");
            Parametros["dbport"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:dbport");
            Parametros["locale"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:locale");
            Parametros["protocol"] = _config.GetValue<string>("ParametrosAcx:ConnecConnectionStringsPadraotionStrings:protocol");
            Parametros["server"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:server");
            Parametros["sid"] = _config.GetValue<string>("ParametrosAcx:ConnectionStringsPadrao:sid");

            Parametros["reenviar_notif_erro"] = _config.GetValue<string>("ParametrosAcx:Parametros:reenviar_notif_erro");
            Parametros["ativar_notificacoes"] = _config.GetValue<string>("ParametrosAcx:Parametros:ativar_notificacoes");
            Parametros["ativar_log_query"] = _config.GetValue<string>("ParametrosAcx:Parametros:ativar_log_query");
            Parametros["ativar_log"] = _config.GetValue<string>("ParametrosAcx:Parametros:ativar_log");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
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

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/healthz").RequireAuthorization();
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

        }
    }
}
