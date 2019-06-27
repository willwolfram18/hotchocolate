using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StarWars.Data;
using StarWars.Types;

namespace StarWars
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment)
        {
            Environment = environment;
        }
        
        public IHostingEnvironment Environment { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the custom services like repositories etc ...
            services.AddSingleton<CharacterRepository>();
            services.AddSingleton<ReviewRepository>();

            // Add in-memory event provider
            services.AddInMemorySubscriptionProvider();

            // Add GraphQL Services
            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddServices(sp)

                // Adds the authorize directive and
                // enable the authorization middleware.
                .AddAuthorizeDirectiveType()

                .AddQueryType<QueryType>()
                .AddMutationType<MutationType>()
                .AddSubscriptionType<SubscriptionType>()
                .AddType<HumanType>()
                .AddType<DroidType>()
                .AddType<EpisodeType>()
                .Create(),
                new QueryExecutionOptions
                {
                    TracingPreference = TracingPreference.Always
                });


            // Add Authorization Policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasCountry", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            (c.Type == ClaimTypes.Country))));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseWebSockets()
                .UseGraphQL(new QueryMiddlewareOptions
                {
                    Path = "/graphql",
                    OnCreateRequest = (c, r, p) =>
                    {
                        // Normally would include a Environment.IsDevelopment() here as well.
                        if (c.User is IPrincipal user &&
                            !user.Identity.IsAuthenticated)
                        {
                            // Overwrite the context's user with one that is authenticated
                            // for local development.
                            var claimsIdentity = new ClaimsIdentity("LocalDevelopment");
                            c.User = new ClaimsPrincipal(claimsIdentity);
                        }
                        return Task.CompletedTask;
                    }
                })
                .UseGraphiQL("/graphql")
                .UsePlayground("/graphql")
                .UseVoyager("/graphql");

            /*
            Note: comment app.UseGraphQL("/graphql"); and uncomment this
            section in order to simulate a user that has a country claim and
            passes the configured authorization rule.

            .UseGraphQL(new QueryMiddlewareOptions
            {
                Path = "/graphql",
                OnCreateRequest = (ctx, builder, ct) =>
                {
                    var identity = new ClaimsIdentity("abc");
                    identity.AddClaim(new Claim(ClaimTypes.Country, "us"));
                    ctx.User = new ClaimsPrincipal(identity);
                    builder.SetProperty(nameof(ClaimsPrincipal), ctx.User);
                    return Task.CompletedTask;
                }
            })
            */
        }
    }
}
