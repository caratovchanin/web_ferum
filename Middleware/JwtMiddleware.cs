namespace webFerum.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        public JwtMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Session.GetString("accessToken");



            if (context.Request.Path == "/logout")
            {
                context.Session.Remove("accessToken");
                
                context.Response.Redirect("/"); 
                return;
            }

            if (token is not null)
            {
                context.Request.Headers.Authorization = "Bearer " + token;
            }

            await next.Invoke(context);
        }
    }
}
