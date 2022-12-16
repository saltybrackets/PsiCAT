namespace PsiCat.Home
{
    using Microsoft.AspNetCore.Authentication.Negotiate;
    using Microsoft.AspNetCore.HttpOverrides;
    using NLog;
    using NLog.Web;
    using PsiCat;
    using PsiCat.SmartDevices;


    public class Program
    {
        private static PsiCatClient psiCatClient;
        private static Logger logger;

        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            SetupLogging(builder);
            
            psiCatClient = new PsiCatClient();
            StartPsiCatServices();
            
            RegisterServices(builder);
            
            WebApplication webApplication = builder.Build();
            ConfigureWebApplication(webApplication);

            await webApplication.RunAsync();
        }


        private static void SetupLogging(WebApplicationBuilder builder)
        {
            NLog.LogManager.Setup()
                .LoadConfiguration(builder => {
                        builder.ForLogger()
                            .FilterMinLevel(LogLevel.Info)
                            .WriteToConsole();
                        builder.ForLogger()
                            .FilterMinLevel(LogLevel.Info)
                            .WriteToFile(fileName: "psicat-home.log");
                    });
            
            logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();
            
            builder.Logging.AddConsole();
            builder.Host.UseNLog();
        }


        private static void StartPsiCatServices()
        {
            psiCatClient.Logger = new PsiCatHomeLogger(logger);
            psiCatClient.LoadPlugins();
            psiCatClient.LoadConfig();
            psiCatClient.LoadInternalCommands();
            psiCatClient.Start();
        }
        

        private static void AddAuthenticationService(WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();

            builder.Services.AddAuthorization(
                options =>
                    {
                        // By default, all incoming requests will be authorized according to the default policy.
                        options.FallbackPolicy = options.DefaultPolicy;
                    });
        }


        private static void ConfigureWebApplication(WebApplication webApplication)
        {
            // Configure the HTTP request pipeline.
            if (!webApplication.Environment.IsDevelopment())
            {
                webApplication.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                webApplication.UseHsts();
            }

            // webApplication.UseHttpsRedirection();
            webApplication.UseStaticFiles(
                new StaticFileOptions
                    {
                        ServeUnknownFileTypes = true,
                        DefaultContentType = "application/octet-stream"
                    });
            webApplication.UseStaticFiles();
            webApplication.UseRouting();
            webApplication.MapBlazorHub();
            webApplication.MapFallbackToPage("/_Host");
            webApplication.UseForwardedHeaders(
                new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                    });

            // Authentication
            //webApplication.UseAuthentication();
            //webApplication.UseAuthorization();
            webApplication.MapControllers();
        }


        // Add services to the container.
        //   AddSingleton for global services.
        //   AddScoped for user-specific services.
        private static void RegisterServices(WebApplicationBuilder builder)
        {
            SmartDevicesPlugin smartDevices = (SmartDevicesPlugin)psiCatClient.Plugins["PsiCAT Smart Devices"];
            
            // PsiCAT Services
            builder.Services.AddSingleton(psiCatClient);
            builder.Services.AddSingleton(smartDevices);
            
            // Blazor Services
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            
            // API Endpoints
            builder.Services.AddControllers();

            // Authentication
            //AddAuthenticationService(builder);
        }
    }
}


