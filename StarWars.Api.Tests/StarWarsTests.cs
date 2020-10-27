using System.Collections;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarWars.Api.Tests;
using NGraphQL.Utilities;
using NGraphQL.Server;
using System.Collections.Generic;
using StarWars.Api;

namespace StarWars.Tests {
  using TDict = IDictionary<string, object>;

  [TestClass]
  public class StarWarsTests {
    [TestInitialize]
    public void TestInit() {
      TestEnv.Init();
    }

    [TestMethod]
    public async Task TestBasicQueries() {
      string query;
      GraphQLResponse resp;

      query = @" query { starships{name, length coordinates} } ";
      resp = await TestEnv.ExecuteAsync(query);
      var ships = resp.Data.GetValue<IList>("starships");
      Assert.AreEqual(4, ships.Count, "expected 4 ships");

      query = @" query { starship(id: ""3001"") {name, length} } ";
      resp = await TestEnv.ExecuteAsync(query);
      var shipName = resp.Data.GetValue<string>("starship.name");
      Assert.AreEqual("X-Wing", shipName);

      // character query with friends
      query = @" {
  leia: character(id: ""1003"") { 
    name
    friends { name }
  }
}";
      resp = await TestEnv.ExecuteAsync(query);
      var lname = resp.Data.GetValue<string>("leia.name");
      Assert.AreEqual("Leia Organa", lname);
      var leiaFriends = resp.Data.GetValue<IList>("leia.friends");
      Assert.AreEqual(4, leiaFriends.Count, "Expected 4 friends");

    }

    [TestMethod]
    public async Task TestBatching() {
      string query;
      GraphQLResponse resp;
      // characters with starships (on humans only)
      query = @" {
  charList: characters(episode: JEDI) { 
    name
    ... on Human { 
      starships { name }
    }    
  }
}";
      StarWarsResolvers.CallCount_GetStarships = 0; //reset the counter
      resp = await TestEnv.ExecuteAsync(query);
      var charList = resp.Data.GetValue<IList>("charList");
      Assert.IsTrue(charList.Count > 0);
      Assert.AreEqual(1, StarWarsResolvers.CallCount_GetStarships, "Expected 1 call to resolver");
    }

    [TestMethod]
    public async Task TestMutation() {
      GraphQLResponse resp;

      // get Jedi reviews, add review, check new count
      // 1. Get Jedi reviews, get count
      var getReviewsQuery = @"
{
  reviews( episode: JEDI) { episode, stars, commentary, emojis }
}";
      resp = await TestEnv.ExecuteAsync(getReviewsQuery);
      var jediReviews = resp.Data.GetValue<IList>("reviews");
      Assert.IsTrue(jediReviews.Count > 0, "Expected some review");
      var oldReviewsCount = jediReviews.Count;

      // 2. Add review for Jedi
      var createReviewMut = @"
mutation {
  createReview( episode: JEDI, reviewInput: { stars: 2, commentary: ""could be better"", emojis: [DISLIKE, BORED]}) 
    { episode, stars, commentary, emojis }
}";
      resp = await TestEnv.ExecuteAsync(createReviewMut);

      // 3. Get reviews again and check count
      resp = await TestEnv.ExecuteAsync(getReviewsQuery);
      jediReviews = resp.Data.GetValue<IList>("reviews");
      var newReviewsCount = jediReviews.Count;
      Assert.AreEqual(oldReviewsCount + 1, newReviewsCount, "Expected incremented review count");
    }

    [TestMethod]
    public async Task TestSearch() {
      string query;
      GraphQLResponse resp;

      query = @" 
query { 
  search (text: ""er"") {
    __typename,
    name, 
  } 
} ";
      resp = await TestEnv.ExecuteAsync(query);
      var results = resp.Data.GetValue<IList>("search");
      // Luke SkywalkER, Darth VadER, ImpERial shuttle
      Assert.AreEqual(3, results.Count, "expected 3 objects");
    }

  }
}
