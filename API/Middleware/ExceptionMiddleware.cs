using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using System.Net;
using API.Errors;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
    
            private readonly RequestDelegate m_Next;
            private readonly ILogger<ExceptionMiddleware> m_Logger;
            private readonly IHostEnvironment m_Environment;

            //RequestDelegate - What's coming up next in the middleewre pipeline
            //ILogger - Print to terminal
            //IHostEnvironment - Checking the environment running: development or  production...
        public ExceptionMiddleware(RequestDelegate i_Next
                                 , ILogger<ExceptionMiddleware> i_Logger
                                 , IHostEnvironment i_Environment)
        {
            
            m_Next = i_Next;
            m_Environment = i_Environment;
            m_Logger = i_Logger;

        }

        //Give this middleware its required method
        public async Task InvokeAsync(HttpContext context)//HttpContext - This is happening in the context of an HTTP request when we're add where we have access to the actual HTTP request that's coming in.
        {

                try
                {

                    await m_Next(context);//RequestDelegate -> TYPE: 'public delegate Task RequestDelegate(HttpContext context);'

                }
                catch(Exception ex)
                {

                    m_Logger.LogError(ex, ex.Message);//This is not the best solution because we cant see any details
                    context.Response.ContentType = "applecation/json";//Format
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;//Status code

                    //Create response
                    var response = m_Environment.IsDevelopment()
                    //What happend if might break the Internet?
                    //so we going to exception because the '?' mark
                    ? new ApiException(context.Response.StatusCode, ex.Message , ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internet server error");
                    //ex.StackTrace - A string that describes the immediate frames of the call stack.
                
                    //Create respnse in JSON file, and this is save about our options
                    //Pass the our option and we create the json serialzer and checking if is equal to json
                    var optionsResponseInFormatJSON = new JsonSerializerOptions
                    {
                      
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase//The naming policy for camel-casing.
                    
                    };

                    var json = JsonSerializer.Serialize(response, optionsResponseInFormatJSON);
               
                    await context.Response.WriteAsync(json);
                }

        }
    }
}