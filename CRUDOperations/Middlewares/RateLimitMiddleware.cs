namespace CRUDOperations.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        
        private static int _counter = 0;
        private static DateTime _lastRequest = DateTime.Now;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // limit the requestes to be 5 requests in 10 Seconds only
        public async Task InvokeAsync(HttpContext context)
        {
            _counter++;
            if (DateTime.Now.Subtract(_lastRequest).Seconds > 10)
            {
                _counter = 1;
                _lastRequest = DateTime.Now;
                await _next(context);
            }
            else
            {
                if (_counter > 5)
                {
                    _lastRequest = DateTime.Now;
                    // write on response and return it back
                    await context.Response.WriteAsync("Rate limit existed");
                }
                else
                {
                    _lastRequest = DateTime.Now;
                    await _next(context);
                }
            }
        }
    }
}
