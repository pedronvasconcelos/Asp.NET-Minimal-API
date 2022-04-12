using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                            options.UseMySql(connectionString,
                            ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

var app = builder.Build();

//endpoint
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

//endpoints
app.MapGet("/", () => "Catalog API - A minimal api project").ExcludeFromDescription().RequireAuthorization();
app.MapPost("/categories", async (Category category, AppDbContext db) =>
{
    await db.Categories.AddAsync(category);
    await db.SaveChangesAsync();

    return Results.Created($"/categories/{category.CategoryId}", category);
});

app.MapGet("/categories", async (AppDbContext db) =>
{
    var categories = await db.Categories.ToListAsync();
    return Results.Ok(categories);
});

app.MapGet("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id)
        is Category category
            ? Results.Ok(category)
            : Results.NotFound();
});

app.MapPut("/categories/{id:int}", async (int id, Category category, AppDbContext db) =>
{
    if (category.CategoryId != id)
    {
        return Results.BadRequest();
    }
    var categoryDB = await db.Categories.FindAsync(id);

    if (categoryDB is null)
    {
        return Results.NotFound();
    }

    categoryDB.Name = category.Name;
    categoryDB.Description = category.Description;

    await db.SaveChangesAsync();
    return Results.Ok(categoryDB);
   
});

app.MapDelete("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);

    if (category is null)
    {
        return Results.NotFound();
    }

    db.Categories.Remove(category);
    await db.SaveChangesAsync();
    return Results.Ok();
});

//endpoints for products
app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    await db.Products.AddAsync(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.ProductId}", product);
});

app.MapGet("/products", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Products.FindAsync(id)
        is Product product
            ? Results.Ok(product)
            : Results.NotFound();
});

app.MapPut("/products/{id:int}", async (int id, Product product, AppDbContext db) =>
{
    if (product.ProductId != id)
    {
        return Results.BadRequest();
    }
    var productDB = await db.Products.FindAsync(id);

    if (productDB is null)
    {
        return Results.NotFound();
    }

    productDB.Name = product.Name;
    productDB.Description = product.Description;
    productDB.Price = product.Price;
    productDB.CategoryId = product.CategoryId;
    productDB.Stock = product.Stock;
    productDB.Image = product.Image;
    productDB.DatePurchase = product.DatePurchase;
    

    await db.SaveChangesAsync();
    
    return Results.Ok(productDB);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();


