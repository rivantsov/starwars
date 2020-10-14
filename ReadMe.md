## NGraphQL StarWars Example
StarWars is a sample project implementing StarWars-themed API widely used in GraphQL documentation and guides. The project demonstrates building GraphQL APIs in .NET/c# using NGraphQL library: https://github.com/rivantsov/ngraphql . 

The GraphQL API is described by the [schema document StarWars.schema](https://github.com/rivantsov/starwars/blob/master/StarWars.schema) in the root folder. 

### Exploring the code
Open the solution in Visual Studio, compile, run tests in StarWars.Api.Tests project. The tests write full log of operations into **_starWarsTests.log** - request, response as Json. After running the tests find this file in the bin/../ folder, open it in text editor and what tests are doing - request/response, etc. 

You can start Http server and explore the API in **Graphiql UI**. Set the project **StarWars.HttpServer** as startup project and start it from Visual Studio. It is a concole app that starts the HTTP server with GraphQL endpoint.

Locate the [Graphiql.html](https://github.com/rivantsov/starwars/blob/master/Graphiql/graphiql.html) file on disk and open it in browser. You can execute queries from the Graphiql environment, some sample queries are in *SampleQueries.txt* file.  

Projects/Folders: 
* **StarWars** - the StarWars application, defines business entities and access to methods and repositories. Defines entities like Human, Droid, Character (interface), Starship etc. 
* **StarWars.Api** - GraphQL API running on top of StarWars app; exposing it to external clients. Defines the API using .NET classes and interfaces.
* **StarWars.Api.Tests** - unit tests for the API.
* **StarWars.HttpServer** - a console app running HTTP server exposing StarWars API over Http. 
* **Graphiql** - folder containing Graphiql UI page, configured to connect to the running Http server. 

### System Requirements
.NET Standard 2.0, Visual Studio 2019


