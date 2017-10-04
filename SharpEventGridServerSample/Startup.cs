using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpEventGridServer;
using System.Net.Http;

namespace SharpEventGridServerSample {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddSingleton(new HttpClient());
            services.AddSingleton(new SomeEventHandler());
            services.AddSingleton<IDatabase, Database>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            var opt = new EventGridOptions();
            opt.AutoValidateSubscription = true;
            opt.EventsPath = "api/events";
            opt.MapEvent<SomeEventHandler>("someEventType");
            opt.MapEvent<NewCustomerEventHandler>("newCustomerEventType");
            opt.MapDefault<DefaultMappingHandler>();
            opt.SetValidationKey("key", "foo");
            app.UseEventGrid(opt);

            app.UseMvc();
        }
    }
}
