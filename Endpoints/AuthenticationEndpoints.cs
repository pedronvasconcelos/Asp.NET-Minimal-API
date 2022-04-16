using CatalogAPI.Models;
using CatalogAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogAPI.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null)
                {
                    return Results.BadRequest("Invalid Login");
                }
                if (userModel.UserName == "nantes" && userModel.Password == "nantes#1234")
                {
                    var tokenString = tokenService.GenerateToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        userModel);
                    return Results.Ok(new { token = tokenString });

                }
                else
                {
                    return Results.BadRequest("Invalid Login");
                }
            }).Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status200OK)
            .WithName("Login")
            .WithTags("Authentication");
        }
    }
}
