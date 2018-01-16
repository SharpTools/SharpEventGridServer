using System;
using SharpEventGrid;

namespace SharpEventGridServer
{
    public class EventGridOptions
    {
        protected Type _defaultMapping;

        internal IServiceProvider ServiceProvider { get; set; }

        public string EventsPath { get; set; } = "api/events";
        public bool AutoValidateSubscription { get; set; } = true;
        public Action ValidateSubscriptionCallBack { get; set; }
        public string ValidationKey { get; set; }
        public string ValidationValue { get; set; }
        public Action<string, bool, string> AutoValidateSubscriptionAttemptNotifier { get; set; }
        public EventGridMapper Mapper = new EventGridMapper();
        public virtual void MapEvent<T>(string eventType) where T : IEventGridHandler
        {
            Mapper.AddMapping(eventType, typeof(T));
        }
        public void MapDefault<T>() where T : IEventGridHandler
        {
            _defaultMapping = typeof(T);
        }

        public void SetValidationKey(string key, string value)
        {
            ValidationKey = key;
            ValidationValue = value;
        }

        internal virtual IEventGridHandler ResolveHandler(Event item)
        {
            var typeToCreate = Mapper.LookUpMapping(item);
            if (typeToCreate == null)
                typeToCreate = _defaultMapping;
            var handler = ServiceProvider.GetService(typeToCreate) ?? Activator.CreateInstance(typeToCreate);
            return (IEventGridHandler)handler;
        }
    }
}