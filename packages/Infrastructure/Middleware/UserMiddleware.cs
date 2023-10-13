using System.Security.Claims;
using Infrastructure.Service;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace Infrastructure.Middleware
{
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext userContext)
        {
            var userId = context.User.FindFirst(ClaimTypes.Name)?.Value;

            if (userId != null)
                userContext.UserId = userId;

            await _next(context);
        }
    }
}