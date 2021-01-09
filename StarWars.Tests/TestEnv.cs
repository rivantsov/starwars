﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using NGraphQL.Client;
using NGraphQL.Server;
using NGraphQL.Server.AspNetCore;
using NGraphQL.Utilities;
using StarWars.Api;

namespace StarWars.Tests {
  public static class TestEnv {
    public static string ServiceUrl = "http://127.0.0.1:55000";
    public static string GraphQLEndPointUrl = ServiceUrl + "/graphql";
    public static GraphQLHttpServer StarWarsHttpServer;
    public static GraphQLClient Client;

    public static string LogFilePath = "_starWarsTests.log";
    private static JsonSerializerSettings _serializerSettings;


    static IWebHost _webHost;

    public static void Initialize() {
      if (StarWarsHttpServer != null) //already initialized
        return;
      if (File.Exists(LogFilePath))
        File.Delete(LogFilePath);

      _serializerSettings = new JsonSerializerSettings() {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() }
      };
      _serializerSettings.Converters.Add(new StringEnumConverter()); // enums as strings, not ints

      // create server and Http graphQL server 
      var app = new StarWarsApp();
      var starWarsServer = new GraphQLServer(app);
      starWarsServer.RegisterModules(new StarWarsApiModule());
      starWarsServer.Initialize();
      StarWarsHttpServer = new GraphQLHttpServer(starWarsServer);

      StartWebHost();
      Client = new GraphQLClient(GraphQLEndPointUrl);
      Client.RequestCompleted += Client_RequestCompleted;
    }

    private static void StartWebHost() {
      var hostBuilder = WebHost.CreateDefaultBuilder()
          .ConfigureAppConfiguration((context, config) => { })
          .UseStartup<TestStartup>()
          .UseUrls(ServiceUrl)
          ;
      _webHost = hostBuilder.Build();
      Task.Run(() => _webHost.Run());
      Debug.WriteLine("The service is running on URL: " + ServiceUrl);
    }

    public static void ShutDown() {
      _webHost?.StopAsync().Wait();
    }

    public static void LogTestMethodStart([CallerMemberName] string testName = null) {
      LogText($@"

==================================== Test Method {testName} ================================================
");
    }

    public static void LogTestDescr(string descr) {
      LogText($@"
Testing: {descr}
");
    }


    private static void Client_RequestCompleted(object sender, RequestCompletedEventArgs e) {
      LogCompletedRequest(e.Data);
    }

    public static void LogCompletedRequest(ResponseData response) {
      string reqText;
      var req = response.RequestData;
      if (req.HttpMethod == "GET") {
        reqText = @$"GET, URL: {req.UrlQueryPartForGet} 
                unescaped: {Uri.UnescapeDataString(req.UrlQueryPartForGet)}";
      } else
        reqText = "POST, payload: " + Environment.NewLine + response.RequestData.Body;
      // for better readability, unescape \r\n
      reqText = reqText.Replace("\\r\\n", Environment.NewLine);
      var jsonResponse = JsonConvert.SerializeObject(response.TopFields, Formatting.Indented);
      var text = $@"
Request: 
{reqText}

Response:
{jsonResponse}

//  time: {response.DurationMs} ms
----------------------------------------------------------------------------------------------------------------------------------- 

";
      LogText(text);
      if (response.Exception != null)
        LogText(response.Exception.ToText());
    }


    static object _lock = new object();
    public static void LogText(string text) {
      lock (_lock) {
        File.AppendAllText(LogFilePath, text);
      }
    }

  }
}