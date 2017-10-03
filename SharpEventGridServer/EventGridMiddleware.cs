using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharpEventGrid;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System;

namespace SharpEventGridServer {
    public class EventGridMiddleware {

        private readonly RequestDelegate _next;
        private readonly EventGridOptions _options;
        private readonly string _eventsPath;

        public EventGridMiddleware(RequestDelegate next, IOptions<EventGridOptions> options) {
            _next = next;
            _options = options.Value;
            _eventsPath = _options.EventsPath.TrimStart('/').ToLower();
        }

        public async Task Invoke(HttpContext context) {
            var request = context.Request;
            var path = request.Path.Value.TrimStart('/').ToLower();
            if (!(path == _eventsPath)) {
                await _next(context);
                return;
            }
            if(! await ValidateKey(context)) {
                return;
            }
            await ProcessEvents(context);
            return;
        }

        private async Task<bool> ValidateKey(HttpContext context) {
            var key = _options.ValidationKey;
            if(String.IsNullOrEmpty(key)) {
                return true;
            }
            var value = _options.ValidationValue;
            if (context.Request.Query.ContainsKey(key) &&
                context.Request.Query[key] == value) {
                return true;
            }
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Key");
            return false;
        }

        private async Task ProcessEvents(HttpContext context) {
            var events = await ReadContentBody(context.Request);
            foreach (var item in events) {
                if(item.EventType == EventTypes.SubscriptionValidationEvent && _options.AutoValidateSubscription) {
                    ValidateSubscription(context, item);
                    return;
                }
                var handler = _options.ResolveHandler(item);
                await handler.ProcessEvent(item);
            }
            context.Response.StatusCode = StatusCodes.Status204NoContent;
        }

        private void ValidateSubscription(HttpContext context, Event item) {
            var validationEvent = item.DeserializeEvent<ValidationEvent>();

            var validationResponse = new ValidationEventResponse {
                ValidationResponse = validationEvent.ValidationCode
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;
            var json = JsonConvert.SerializeObject(validationResponse);
            context.Response.WriteAsync(json);
        }

        private async Task<List<Event>> ReadContentBody(HttpRequest request) {
            using (var reader = new StreamReader(request.Body)) {
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Event>>(json);
            }
        }
    }
}