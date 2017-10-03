using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace SharpEventGridServer {
    public static class EventGridMiddlewareExtensions {

        public static IApplicationBuilder UseEventGrid(this IApplicationBuilder app, EventGridOptions proxyOptions) {
            proxyOptions.ServiceProvider = app.ApplicationServices;
            return app.UseMiddleware<EventGridMiddleware>(Options.Create(proxyOptions));
        }
    }
}
