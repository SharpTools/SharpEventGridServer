using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpEventGridServer;

namespace SharpMappingOverrideSample {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddSingleton(new HttpClient());
            services.AddSingleton(new mySubject_SomeEventHandler());
            services.AddSingleton<IDatabase, Database>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            var opt = new EventGridOptions();
            opt.AutoValidateSubscription = true;
            opt.AutoValidateSubscriptionAttemptNotifier = (url, success, message) => {
                Debug.WriteLine($"Validation attempt: {url} -> Success: {success}: {message}");
            };
            opt.EventsPath = "api/events";
            opt.MapEvent<mySubject_SomeEventHandler>("someEventType");
            opt.MapEvent<mySubject_NewCustomerEventHandler>("newCustomerEventType");
            opt.MapDefault<DefaultMappingHandler>();
            opt.SetValidationKey("key", "foo");
            app.UseEventGrid(opt);
            opt.Mapper=new SubjectConventionMapper();
            app.UseMvc();
        }
    }
}
