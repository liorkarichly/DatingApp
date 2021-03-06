using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using API.Extensions;
using API.Middleware;
using API.SignalR;

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

           services.AddApplicationServices(r_Config);//Extentions method

            services.AddControllers();
        
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            });

            //Into our application and ordering dosent really matter inside here
            services.AddCors();

           services.AddIdentityServices(r_Config);//Extentions method
            
            services.AddSignalR();//Add the service of SignalR
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //Checking if were in development mode and if we are and our application encounters a problem
            //Its show in top of scope because if an exception happens anywhere wlse in the middle where or as part of our request 
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            //     app.UseSwagger();
            //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            // }
            
            app.UseMiddleware<ExceptionMiddleware>();
            
            //If we did come in on a HTTP address, then we get redirected to the HTTPS endpoints
            app.UseHttpsRedirection();

            //We've seen routing and action because we were able to go from our browser,
            // the weather forecast endpoint and get to our forecast controller
            app.UseRouting();

            //Take some configuration
            app.UseCors(policy => policy.AllowAnyHeader()// allow to any headers in policy, sending up headers such as authentication headers to our API from our angular application
                                         .AllowAnyMethod()//allow in any method in policy,to allow put requset, post request, get request,etc.
                                         .AllowCredentials()//Supply lists when we're using signalR due to the way that we now send up our access token or our token.
                                         .WithOrigins("https://localhost:4200"));//the origin that we want to route (specific origin) 

            //This so important to Authentication before the Authorization
            app.UseAuthentication();

            //Which isnt doing much for us at the moment because we haven't configured any authorization
            app.UseAuthorization();

            /*
            say app use default files, first of all.
            And what this will do is if there's an index to age HTML inside there, then it's going to use that
            */
            app.UseDefaultFiles();

            //Because the Angular service
            app.UseStaticFiles();

            //We've got the middleware to actually use the endpoints and we've got a method
            //look inside to endpoints here to map the controllers
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");//President's hub going to be accessed from
                endpoints.MapHub<MessageHub>("hubs/message");
                endpoints.MapFallbackToController("Index", "Fallback");
                
            });

        }

    }

}
