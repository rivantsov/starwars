## Welcome to GraphQL on .NET!
[NGraphQL](https://github.com/rivantsov/ngraphql) is a framework for building **GraphQL** servers on .NET platform.

## NGraphQL StarWars Example
StarWars is a sample project implementing StarWars-themed API widely used in GraphQL documentation and guides. The project demonstrates building GraphQL APIs in .NET/c# using [NGraphQL library](https://github.com/rivantsov/ngraphql). 

Documentation: [StarWars sample developer guide](https://github.com/rivantsov/starwars/wiki)

The GraphQL API is described by this [Star Wars schema document](https://github.com/rivantsov/starwars/blob/master/StarWars.schema) in the root folder. 

### Exploring the code
Open the solution in Visual Studio, compile, run tests in StarWars.Api.Tests project. The tests write full log of operations into **_starWarsTests.log** - request, response as Json. After running the tests find this file in the *bin/../ folder*, open it in text editor and follow the test actions - request/response, etc. 

You can start an HTTP Server and explore the API in **Graphiql UI**. Set the project **StarWars.HttpServer** as a startup project and run it from Visual Studio. It is a console app that starts the HTTP server with a _GraphQL_ endpoint.

Locate the [Graphiql.html](https://github.com/rivantsov/starwars/blob/master/Graphiql/graphiql.html) file on disk and open it in the browser (Chrome is recommended). You can execute queries interactively and see the responses inside the UI; some sample queries are in the *SampleQueries.txt* file.  

Projects/folders: 
* **StarWars** - the StarWars application, defines business entities, app object that holds actual collections of objects. Defines entities like Human, Droid, Character (interface), Starship etc. 
* **StarWars.Api** - GraphQL API running on top of StarWars app. Defines the GraphQL API using .NET classes and interfaces.
* **StarWars.Api.Tests** - unit tests for the API.
* **StarWars.HttpServer** - a console app running HTTP server exposing StarWars API over Http. 
* **Graphiql** - a folder containing the _Graphiql UI_ page, configured to connect to the running Http server. 

### System Requirements
.NET Standard 2.0, Visual Studio 2019


