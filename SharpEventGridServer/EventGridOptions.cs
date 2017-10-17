using System;
using System.Collections.Generic;
using SharpEventGrid;

namespace SharpEventGridServer {
    public class EventGridOptions {
        private Type _defaultMapping;
        private Dictionary<string, Type> _handlers = new Dictionary<string, Type>();
        internal IServiceProvider ServiceProvider { get; set; }

        public string EventsPath { get; set; } = "api/events";
        public bool AutoValidateSubscription { get; set; } = true;
        public Action ValidateSubscriptionCallBack { get; set; }
        public string ValidationKey { get; set; }
        public string ValidationValue { get; set; }
        public Action<string, bool, string> AutoValidateSubscriptionAttemptNotifier { get; set; }

        public void MapEvent<T>(string eventType) where T : IEventGridHandler {
            _handlers.Add(eventType.ToLower(), typeof(T));
        }
        public void MapDefault<T>() where T : IEventGridHandler {
            _defaultMapping = typeof(T);
        }

        public void SetValidationKey(string key, string value) {
            ValidationKey = key;
            ValidationValue = value;
        }

        internal IEventGridHandler ResolveHandler(Event item) {
            var key = item.EventType.ToLower();
            Type typeToCreate = _handlers.ContainsKey(key) ? _handlers[key] : _defaultMapping;
            var handler = ServiceProvider.GetService(typeToCreate) ?? Activator.CreateInstance(typeToCreate);
            return (IEventGridHandler)handler;
        }
    }
}