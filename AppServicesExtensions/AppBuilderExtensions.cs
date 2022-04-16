namespace CatalogAPI.AppServicesExtensions
{
    public static class AppBuilderExtensions
    {
        
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, 
            IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .WithMethods("Get")
                .AllowAnyHeader()); 
            return app;
        }

        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
            });
            return app;
        }
    }
}
