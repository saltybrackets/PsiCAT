namespace PsiCat.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using global::PsiCat.Plugins;


    public class Startup
    {
        private PluginHost pluginHost;


        public Startup(IConfiguration configuration)
        {
            LoadPlugins();

            this.Configuration = configuration;
            SetupUserData();
        }


        public IConfiguration Configuration { get; }


        private void LoadPlugins()
        {
            this.pluginHost = new PluginHost();
        }


        private void SetupUserData()
        {
            using (UserDataContext client = new UserDataContext())
            {
                //Create the database file.
                client.Database.EnsureCreated();
                //Create the database tables.
                client.Database.Migrate();
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DB Contexts.
            services.AddDbContext<UserDataContext>(
                options => options.UseSqlite(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}