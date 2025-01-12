using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StealCat.Data;
using StealCat.Repositories.Interfaces;
using StealCat.Repositories.Repositories;
using StealCatApi.Services;

namespace StealCatApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string caasApiKey = Configuration["CaaSApi:ApiKey"];
            string caasApiBaseUrl = Configuration["CaaSApi:BaseUrl"];

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cats API", Version = "v1" });
            });

            //Register Services
            services.AddScoped<CatsService>();
            services.AddScoped<ValidationService>();

            //Register Repositories
            services.AddScoped<ICatRepository, CatRepository>();
            services.AddScoped<ITagRepository, TagRepository>();

            // Register HttpClientFactory
            services.AddHttpClient(); 

            services.AddDbContext<CatDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                )
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cat API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CatDbContext>();
                dbContext.Database.Migrate();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}