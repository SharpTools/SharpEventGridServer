using System;
using System.Collections.Generic;
using System.Linq;
using SharpEventGrid;
using SharpEventGridServer;

namespace SharpMappingOverrideSample
{
    public class SubjectConventionMapper : EventGridMapper
    {
        private readonly Dictionary<string, List<Type>> handlers = new Dictionary<string, List<Type>>();

        public override void AddMapping(string eventType, Type type)
        {
            if (handlers.Any(h => h.Key == eventType.ToLower()))
                handlers[eventType].Add(type);
            else
                handlers.Add(eventType.ToLower(), new List<Type> { type });
        }

        public override Type LookUpMapping(Event item)
        {
            var key = item.EventType.ToLower();
            if (handlers.ContainsKey(key) && handlers[key].Any(t => t.Name.StartsWith(item.Subject + "_")))
                return handlers[key].First(t => t.Name.StartsWith(item.Subject + "_"));
            return null;
        }
    }
}
