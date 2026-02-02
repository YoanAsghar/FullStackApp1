namespace Routes
{
    public static class ProductEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/products/NewProduct", () =>
            {
                return "Product created";
            });


        }

    }

}

