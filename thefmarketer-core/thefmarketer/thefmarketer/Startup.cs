﻿using Fmarketer.DataAccess.Repository;
using Fmarketer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace thefmarketer
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddDbContext<MainContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<AdminRepository, AdminRepository>();
            services.AddScoped<ChatRepository, ChatRepository>();
            services.AddScoped<ConsultantCoverageRepository, ConsultantCoverageRepository>();
            services.AddScoped<ConsultantRepository, ConsultantRepository>();
            services.AddScoped<ConsultantServiceRepository, ConsultantServiceRepository>();
            services.AddScoped<RequestRepository, RequestRepository>();
            services.AddScoped<ReviewRepository, ReviewRepository>();
            services.AddScoped<SecurityTokenRepository, SecurityTokenRepository>();
            services.AddScoped<UserRepository, UserRepository>();
            services.AddScoped<CredentialRepository, CredentialRepository>();

            services.AddScoped<UnitOfWork, UnitOfWork>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
