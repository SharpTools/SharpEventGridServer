# SharpEventGridServer
Receiving EventGrid Events via WebHook in your application? This is your library.

From Microsoft:

>Azure Event Grid is an innovative offering that makes an event a first-class object in Azure. With Azure Event Grid, you can subscribe to any event that is happening across your Azure resources and react using serverless platforms like Functions or Logic Apps. In addition to having built-in publishing support for events with services like Blob Storage and Resource Groups, Event Grid provides flexibility and allows you to create your own custom events to publish directly to the service.

This library simplify the receiving of events in your ASP.NET application.

## Nuget

```
Install-Package sharpeventgridserver
```

## How to use

After adding the nuget package, open your `Startup` class and go to the `Configure` method:

```cs
var opt = new EventGridOptions();
//Automatically validate Azure EventGrid subscription confirmation event
//to prove that you own the subscriber
opt.AutoValidateSubscription = true;

//Callback for Azure Event Grid validation attempts (debugging)
opt.AutoValidateSubscriptionAttemptNotifier = (url, success, message) => {
    Debug.WriteLine($"Validation attempt: {url} -> Success: {success}: {message}");
};

//Path to receive your events
opt.EventsPath = "api/events";

//Map each kind of event to an IEventGridHandler class 
opt.MapEvent<SomeEventHandler>("someEventType");
opt.MapEvent<SomeOtherEventHandler>("someOtherEventType");

//Any event type without a mapping will be mapped to the default 
//handler specified here
opt.MapDefault<DefaultMappingHandler>();

//If you decided to add a key to every Azure EventGrid request
//to certify its authenticity, set the key name and value here
//so SharpEventGridServer will automatically verify it for you
opt.SetValidationKey("key", "foo");

//Finally, add SharpEventGridServer middleware to your pipeline
app.UseEventGrid(opt);
```

And that's it: configure Azure EventGrid to send events to https://[yourserver]/api/events and you're all set.

## EventGrid Event Handlers

Every EventGrid event handler has to implement the `IEventGridHandler` interface.
Ex:
```cs
public class SomeEventHandler : IEventGridHandler {
    public async Task ProcessEvent(Event eventItem) {
        //do something
    }
}
```
An **IEventGridHandler** is instanciated calling its default constructor every time a new event arrives for it. If you want to use the same instance or have parameters in its constructor, register it in the default asp.net DI.

## Dependency Injection on EventGrid Event Handlers

When mapping event handlers, **SharpEventGridServer** will call the default service locator
to instanciate the required object.

Ex: **Startup.cs**
```cs
public void ConfigureServices(IServiceCollection services) {
    services.AddSingleton<NewCustomerEventHandler>();
    services.AddSingleton<IDatabase, Database>();
    services.AddMvc();
}
```

**NewCustomerEventHandler.cs**
```cs
public class NewCustomerEventHandler : IEventGridHandler {
	private IDatabase _database;
	public NewCustomerEventHandler(IDatabase database) {
		_database = database;
	}

	public async Task ProcessEvent(Event eventItem) {
		var newCustomerEvent = eventItem.DeserializeEvent<NewCustomerEvent>();
		await _database.SaveAsync(newCustomerEvent);
	}
}
```
## Custom mapping
To use a custom mapping, create your own implementation of ShartEventGridMapper.
The following mapper, will map based on a convention that the eventhandler must start with {eventsubject}_

```cs
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
```

To use the new mapper, add it to the EventGridOptions

```cs
opt.Mapper=new SubjectConventionMapper();
```

