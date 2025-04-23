using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Syncfusion.Blazor;
using MatBlazor;
using SICEM_Blazor.Models;
using SICEM_Blazor.Services;
using SICEM_Blazor.Data;
using SICEM_Blazor.Data.Filtros;
using SICEM_Blazor.Recaudacion.Data;
using SICEM_Blazor.Eficiencia.Data;
using SICEM_Blazor.Helpers;
using SICEM_Blazor.Pages.AnalisisInformacion;
using SICEM_Blazor.SeguimientoCobros.Data;
using SICEM_Blazor.Services.Whatsapp;
using SICEM_Blazor.Ordenes.Data;

namespace SICEM_Blazor {
    public class Startup {

        public IConfiguration Configuration { get; }

        private readonly CultureInfo[] supportedCultures;
        
        public Startup(IConfiguration configuration) {
            Configuration = configuration;

            supportedCultures = new[]{
                new CultureInfo("es-MX")
            };
        }


        // This method gets1 called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services){
            
            services.Configure<RequestLocalizationOptions>( options => {
                options.DefaultRequestCulture = new RequestCulture("es-MX");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            
            services.AddRazorPages();
            services.AddMatBlazor();
            services.AddControllers();
            services.AddBlazorBootstrap();
            
            services.AddDbContext<SicemContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("SICEM"));
            });

            services.AddLogging( config => {
                config.AddConsole();
                config.AddEventSourceLogger();
                if(OperatingSystem.IsWindows()){
                    config.AddEventLog( econfig => {
                        #pragma warning disable CA1416 // Validate platform compatibility
                        econfig.LogName = "SolucionesNerus";
                        econfig.SourceName = "SicemV5";
                        #pragma warning restore CA1416 // Validate platform compatibility
                    });
                }
            });

            services.Configure<BingMapsSettings>(Configuration.GetSection("BingMapsSettings"));
            services.AddSingleton<SessionService>();
            services.AddScoped<SicemService>();
            services.AddScoped<UsersSicemService>();
            services.AddScoped<ConsultaGralService>(s => new ConsultaGralService(Configuration, s.GetService<SicemService>()));
            services.AddScoped<IAtencionService, AtencionService>();
            services.AddScoped<IRecaudacionService, RecaudacionService>();
            services.AddScoped<ControlRezagoService>();
            services.AddScoped<MicromedicionService>();
            services.AddScoped<DescuentosService>();
            services.AddScoped<IEficienciaService, EficienciaService>();
            services.AddScoped<FacturacionService>();
            services.AddScoped<OrdenesService>();
            services.AddScoped<PadronService>();
            services.AddSingleton<SICEM_Blazor.Lecturas.Data.LecturasService>();
            services.AddScoped<ConceptosService>();
            services.AddScoped<LogeadoFilter>();
            services.AddScoped<PoblacionesService>();
            services.AddScoped<SucursalesService>();
            services.AddScoped<AnalisisInfoMapJsInterop>();
            services.AddScoped<MapJsInterop>();
            services.AddSicemIncomeOfficeServices();
            services.AddWhatsappService(Configuration);
            services.AddHttpClient("notificacionApi", client => {
                client.BaseAddress = new Uri("http://nerus.sytes.net:3001/lead");
            });
            
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();
            services.AddMatToaster(config =>{
                config.Position = MatToastPosition.TopCenter;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 4000;
            });
            
            services.AddCors(policy => {
                policy.AddPolicy("CorsPolicy", opt => opt
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            // Enable localization middleware
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense( "MzcwNkAzMTM5MmUzMjJlMzBicjZJUjMvTzNzVWZuNFk3Y05EMHZ0WGZuMFFpZGNHOVV4MGZMcVk3L1RRPQ==" );

            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
