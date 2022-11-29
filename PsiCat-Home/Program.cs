using Microsoft.AspNetCore.Authentication.Negotiate;
using PsiCat;
using PsiCat.Home;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

WebApplication app = builder.Build();

PsiCatClient psiCatClient = new PsiCatClient();
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
