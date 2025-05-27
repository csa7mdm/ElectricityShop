using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ElectricityShop.API.Middleware
{
    /// <summary>
    /// Middleware for adding security headers to responses
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            
            // X-Content-Type-Options
            // Prevents MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            
            // X-Frame-Options
            // Prevents clickjacking by not allowing the page to be embedded in iframes
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            
            // X-XSS-Protection
            // Prevents cross-site scripting attacks
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            
            // Referrer-Policy
            // Controls how much referrer information is included with requests
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Content-Security-Policy
            // Defines approved sources of content that the browser may load
            context.Response.Headers.Append(
                "Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
                "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none'; " +
                "form-action 'self'; " +
                "base-uri 'self';"
            );
            
            // Permissions-Policy (formerly Feature-Policy)
            // Controls which browser features can be used
            context.Response.Headers.Append(
                "Permissions-Policy",
                "camera=(), " +
                "microphone=(), " +
                "geolocation=(), " +
                "payment=(), " +
                "usb=(), " +
                "accelerometer=(), " +
                "gyroscope=()"
            );
            
            // Strict-Transport-Security (HSTS)
            // Forces browser to use HTTPS for future requests
            // Only add this header when the request is over HTTPS
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append(
                    "Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains; preload"
                );
            }

            await _next(context);
        }
    }
}