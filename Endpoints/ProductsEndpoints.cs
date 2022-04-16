using CatalogAPI.Context;
using CatalogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Endpoints
{
    public static class ProductsEndpoints
    {
        public static void MapProductsEndpoints(this WebApplication app)
        {
            app.MapPost("/products", async (Product product, AppDbContext db) =>
            {
                await db.Products.AddAsync(product);
                await db.SaveChangesAsync();

                return Results.Created($"/products/{product.ProductId}", product);
            }).WithTags("Product").RequireAuthorization();

            app.MapGet("/products", async (AppDbContext db) =>
            {
                var products = await db.Products.ToListAsync();
                return Results.Ok(products);
            }).WithTags("Product").RequireAuthorization();

            app.MapGet("/products/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Products.FindAsync(id)
                    is Product product
                        ? Results.Ok(product)
                        : Results.NotFound();
            }).WithTags("Product").RequireAuthorization();

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
            }).WithTags("Product").RequireAuthorization();
        }
    }
}
