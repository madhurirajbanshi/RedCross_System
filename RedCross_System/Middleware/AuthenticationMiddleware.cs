namespace RedCross_System.Middleware
{
	public class AuthenticationMiddleware
	{
		private readonly RequestDelegate _next;

		public AuthenticationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Check if the path starts with /Login (for Login controller actions)
			var path = context.Request.Path.Value;

			if (!string.IsNullOrEmpty(path) && (path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase) || path == "/"))
			{
				// Skip authentication check for Login-related requests or root path
				await _next(context);
				return;
			}

			// Check if the user is authenticated by checking session
			var userId = context.Session.GetString("UserId");

			if (string.IsNullOrEmpty(userId))
			{
				// Redirect to login page if not authenticated
				context.Response.Redirect("/Login/Index");
				return;
			}

			// Continue processing the request if authenticated
			await _next(context);
		}

	}

}
