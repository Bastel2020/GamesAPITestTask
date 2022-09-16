using GamesAPITestTask.Database;
using GamesAPITestTask.Interfaces.Models;
using GamesAPITestTask.Interfaces.Repositories;
using GamesAPITestTask.Models;
using GamesAPITestTask.Repositories;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GamesAPITestTask
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

            services.AddDbContext<DefaultDBContext<DefaultGame, DefaultDeveloper, DefaultGenre>>(
        options => options.UseSqlServer(Configuration.GetConnectionString("Database")));

            services.AddScoped<IGamesRepository, DefaultGameRepository<DefaultGame, DefaultDeveloper, DefaultGenre>>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Games API Test Task",
                    Description = "API ��� ��������� �������. ���� �������� Game, Developer, Genre.\n\t" +
                    "����� ������ CRUD �������� � ������, ������������� � ����� �������� ������ �������������, ������ ��������� �����.\n\t" +
                    "����� �������� ����������� ��� ����, ��������� ���� DeveloperName ��� GenreName ��� ��������/�������������� ����.\n\t" +
                    "DeveloperId � GenreId ����� ��������� ��� DeveloperName � GenreName (����� �� ����� ����������� ��� ������� Id).",
                    Contact = new OpenApiContact
                    {
                        Name = "��� GitHub",
                        Url = new Uri("https://github.com/Bastel2020")
                    }
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GamesAPITestTask v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
