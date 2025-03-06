namespace Sfa.Tl.Find.Provider.Web.Middleware
{
    public class SessionMiddleware
    {
        const string SessionHeaderKey = "X-Session-Id";
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            ISession session = httpContext.Session;

            string? sessionId = session.GetString(SessionHeaderKey);

            bool sessionIdSaved = !string.IsNullOrEmpty(sessionId);
            if (!sessionIdSaved)
            {
                sessionId = Guid.NewGuid().ToString();
                session.SetString(SessionHeaderKey, sessionId);
            }

            httpContext.Response.Headers.Add(SessionHeaderKey, sessionId);

            return _next(httpContext);
        }
    }
}