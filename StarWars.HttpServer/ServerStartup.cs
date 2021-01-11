using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NGraphQL.Server;
using NGraphQL.Server.AspNetCore;
using StarWars.Api;

namespace StarWars.HttpServer {

  public class ServerStartup
  {
    public ServerStartup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseHttpsRedirection();
      app.UseRouting();

      // create server and configure GraphQL endpoints
      var server = CreateGraphQLHttpServer(); 
      app.UseEndpoints(endpoints => {
        endpoints.MapPost("graphql", async context => await server.HandleGraphQLHttpRequestAsync(context));
        endpoints.MapGet("graphql", async context => await server.HandleGraphQLHttpRequestAsync(context));
        endpoints.MapGet("graphql/schema", async context => await server.HandleGraphQLHttpRequestAsync(context));
      });
      // Use GraphiQL UI
      app.UseGraphiQLServer();
    }

    private GraphQLHttpServer CreateGraphQLHttpServer() {
      var app = new StarWarsApp();
      var server = new GraphQLServer(app);
      server.RegisterModules(new StarWarsApiModule());
      return new GraphQLHttpServer(server);
    }

  }
}