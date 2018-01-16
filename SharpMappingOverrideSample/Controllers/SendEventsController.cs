﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpEventGrid;

namespace SharpMappingOverrideSample {
    public class SendEventsController : Controller {
        private readonly HttpClient _client;

        public SendEventsController(HttpClient client) {
            _client = client;
        }

        [HttpGet("api/test/subscription")]
        public async Task<IActionResult> TestSubscription() {
            var response = await SendEvent(EventTypes.SubscriptionValidationEvent, new ValidationEvent { ValidationCode = "foo" });
            return Ok(response);
        }
        
        [HttpGet("api/test/event")]
        public async Task<IActionResult> TestEvent(string eventType, string message) {
            var response = await SendEvent(eventType, message);
            return Ok(response);
        }
        private async Task<string> SendEvent(string eventType, object data) {
            var url = $"{Request.Scheme}://{Request.Host.Value}/api/events?{Request.QueryString}";
            var item = new Event {
                EventType = eventType,
                Subject = "mySubject",
                Data = data
            };
            var json = JsonConvert.SerializeObject(new List<Event> { item });
            var response = await _client.PostAsync(url, new StringContent(json));
            var body = await response.Content.ReadAsStringAsync();
            if (String.IsNullOrEmpty(body)) {
                body = "OK";
            }
            return body;
        }
    }
}
