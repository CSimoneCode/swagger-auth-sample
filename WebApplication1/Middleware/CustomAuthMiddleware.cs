using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Net;

namespace WebApplication1.CustomAuthMiddleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _realm;

        public CustomAuthMiddleware(RequestDelegate next, string realm)
        {
            _next = next;
            _realm = realm;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                if (IsAuthorized(username, password))
                {
                    await _next.Invoke(httpContext);
                    return;
                }
            }
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic";

            if (!string.IsNullOrWhiteSpace(_realm))
            {
                httpContext.Response.Headers["WWW-Authenticate"] += $" _realm=\"{_realm}\"";
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        }

        public bool IsAuthorized(string username, string password)
        {
            return username.Equals("User1", StringComparison.InvariantCultureIgnoreCase)
                && password.Equals("SecretPassword!");
            
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthMiddleware>();
        }
    }
}
