using System;
using System.Collections.Generic;
using System.Linq;
using SharpEventGrid;

namespace SharpEventGridServer
{
    public class EventGridMapper
    {
        private readonly Dictionary<string, Type> handlers = new Dictionary<string, Type>();

        public virtual void AddMapping(string eventType, Type type)
        {
            handlers.Add(eventType.ToLower(), type);
        }

        public virtual Type LookUpMapping(Event item)
        {
            var resulttype = handlers.FirstOrDefault(i => i.Key == item.EventType.ToLower());
            return resulttype.Equals(default(KeyValuePair<string, Type>)) ? null : resulttype.Value;
        }
    }
}
