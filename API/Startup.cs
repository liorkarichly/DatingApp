using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;


namespace API
{
    public class Startup
    {
        
        private readonly IConfiguration r_Config;

        public Startup(IConfiguration i_Config)
        {
            r_Config = i_Config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<DataContext>(options => 
            {

                    options.UseSqlite(r_Config.GetConnectionString("DefaultConnection"));

            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //Checking if were in development mode and if we are and our application encounters a problem
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            //If we did come in on a HTTP address, then we get redirected to the HTTPS endpoints
            app.UseHttpsRedirection();

            //We've seen routing and action because we were able to go from our browser,
            // the weather forecast endpoint and get to our forecast controller
            app.UseRouting();

            //Which isnt doing much for us at the moment because we haven't configured any authorization

            app.UseAuthorization();

            //We've got the middleware to actually use the endpoints and we've got a method
            //look inside to endpoints here to map the controllers
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();

            });

        }

    }

}
