// using Microsoft.AspNetCore.Http;
// using System;
// using System.Threading.Tasks;

// public class AuthenticationMiddleware
// {
//     private readonly RequestDelegate _next;

//     public AuthenticationMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }

//     public async Task Invoke(HttpContext context)
//     {
//         // Check if session middleware is configured
//         if (context.Session != null)
//         {
//             // Check session for authentication
//             if (string.IsNullOrEmpty(context.Session.GetString("id")))
//             {
//                 // Redirect or handle unauthorized access
//                 context.Response.Redirect("/login");
//                 return;
//             }
//         }
//         else
//         {
//             // Handle session not configured error gracefully
//             context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//             await context.Response.WriteAsync("Session middleware is not configured properly.");
//             return;
//         }

//         // Continue to the next middleware or controller
//         await _next(context);
//     }
// }


public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private bool redirected = false;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Kiểm tra xem người dùng đã được xác thực hay chưa
        if (context.Session.GetString("id") == null && !redirected)
        {
            context.Session.Clear();
            context.Response.Cookies.Delete(".AspNetCore.Session");

            // Nếu người dùng chưa được xác thực và chưa được chuyển hướng, chuyển hướng đến trang đăng nhập
            context.Response.Redirect("login");
            redirected = true;
        }
        if (context.Session != null && context.Session.Keys.Contains("cookieExpiration") && !redirected)
        {
            if (DateTime.Parse(context.Session.GetString("cookieExpiration")) < DateTime.Now)
            {
                // Xóa tất cả cookies
                context.Session.Clear();
                context.Response.Cookies.Delete(".AspNetCore.Session");
                context.Response.Redirect("login");
                redirected = true;
            }
        }

        // Nếu người dùng đã được xác thực hoặc đã được chuyển hướng, tiếp tục thực hiện các Middleware khác và chạy tới Controller
        await _next(context);
    }
}
