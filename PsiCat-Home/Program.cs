using Microsoft.AspNetCore.Authentication.Negotiate;
using PsiCat;
using PsiCat.Home;
using PsiCat.SmartDevices;


namespace PsiCat.Home
{
    public class Program
    {
        public static void Main(string[] args)
        {
        

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //   AddSingleton for global services.
            //   AddScoped for user-specific services.
            /*
            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();
            
            builder.Services.AddAuthorization(
                options =>
                    {
                        // By default, all incoming requests will be authorized according to the default policy.
                        options.FallbackPolicy = options.DefaultPolicy;
                    });
            */
            PsiCatClient psiCatClient = new PsiCatClient();
            builder.Services.AddSingleton(psiCatClient);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            WebApplication app = builder.Build();

            // Start PsiCAT
            psiCatClient.Logger = new MicrosoftLoggingAdapter(app.Logger);
            psiCatClient.LoadPlugins();
            psiCatClient.LoadConfig();
            psiCatClient.LoadInternalCommands();
            psiCatClient.Start();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}


