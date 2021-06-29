using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SwaggerAuthMiddleware = WebApplication1.CustomAuthMiddleware.CustomAuthMiddleware;

namespace WebApplication1.Middleware
{
    public static class CustomAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthMiddleware>("");
        }
    }

}
