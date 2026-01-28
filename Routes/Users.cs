namespace Routes
{
    public static class UserEndpoints
    {

        public static void Map(WebApplication app)
        {

            //Endpoint 
            app.MapGet("/users/NewUser", () =>
            {
                return "User created corretcly";
            });
        }
    }
}
