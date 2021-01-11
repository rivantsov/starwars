using System.Collections;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using NGraphQL;
using NGraphQL.Server;
using NGraphQL.Utilities;
using StarWars.Api;
using NGraphQL.Client;
using System.Diagnostics;

namespace StarWars.Tests {
  using TDict = IDictionary<string, object>;

  [TestClass]
  public class StarWarsTests {
    [TestInitialize]
    public void TestInit() {
      TestEnv.Initialize();
    }

    [TestMethod]
    public async Task TestBasicQueries() {
      string query;

      query = " query { starships {id, name, length coordinates} } ";
      var response = await TestEnv.Client.PostAsync(query);
      var ships = response.data.starships;
      Assert.AreEqual(4, ships.Count, "expected 4 ships");
      var ship0Name = ships[0].name;
      Assert.IsNotNull(ship0Name, "expected name");
      foreach(var sh in ships) {
        Trace.WriteLine($"Starship {sh.name}, length: {sh.length}");
      }
      // Strongly typed objects
      var shipArr = response.GetTopField<Starship_[]>("starships");
      Starship_ s0 = shipArr[0];
      Assert.IsNotNull(s0, "Expected starship object.");

      query = @" query ($id: ID) { 
        starship(id: $id) {name, length} 
       } ";
      var vars = new Dictionary<string, object>() { { "id", "3001" } };
      response = await TestEnv.Client.PostAsync(query, vars);
      var shipName = response.data.starship.name;
      Assert.AreEqual("X-Wing", shipName);

      // character query with friends, using variable
      query = @" query($id: ID!) {
  leia: character(id: $id) { 
    name
    friends { name }
  }
}";
      vars = new Dictionary<string, object>();
      vars["id"] = "1003";
      response = await TestEnv.Client.PostAsync(query, vars);
      var lname = response.data.leia.name;
      Assert.AreEqual("Leia Organa", lname);
      var leiaFriends = response.data.leia.friends;
      Assert.AreEqual(4, leiaFriends.Count, "Expected 4 friends");
    }

    [TestMethod]
    public async Task TestBatching() {
      string query;

      // characters with starships (on humans only)
      query = @"
query {
  charList: characters(episode: JEDI) { 
    name
    ... on Human { 
      starships { name }
    }    
  }
}";
      var resp = await TestEnv.Client.PostAsync(query);
      var charList = resp.data.charList;
      Assert.IsTrue(charList.Count >= 4, "at least 4 characters expected"); 
      // there are 4 humans in the list, each has 'starships' field, but there was only one call to the resolver;
      //  the resolver does batched retrieval. 
    }

    [TestMethod]
    public async Task TestMutation() {

      // get Jedi reviews, add review, check new count
      // 1. Get Jedi reviews, get count
      var getReviewsQuery = @"
{
  reviews( episode: JEDI) { episode, stars, commentary, emojis }
}";
      var resp = await TestEnv.Client.PostAsync(getReviewsQuery);
      var jediReviews = resp.data.reviews;
      Assert.IsTrue(jediReviews.Count > 0, "Expected some review");
      var oldReviewsCount = jediReviews.Count;

      // 2. Add review for Jedi
      var createReviewMut = @"
mutation {
  createReview( episode: JEDI, reviewInput: { stars: 2, commentary: ""could be better"", emojis: [DISLIKE, BORED]}) 
    { episode, stars, commentary, emojis }
}";
      resp = await TestEnv.Client.PostAsync(createReviewMut);

      // 3. Get reviews again and check count
      resp = await TestEnv.Client.PostAsync(getReviewsQuery);
      jediReviews = resp.data.reviews;
      var newReviewsCount = jediReviews.Count;
      Assert.AreEqual(oldReviewsCount + 1, newReviewsCount, "Expected incremented review count");
    }

    [TestMethod]
    public async Task TestSearch() {
      string query;

      query = @" 
query { 
  search (text: ""er"") {
    __typename,
    name, 
  } 
} ";
      var resp = await TestEnv.Client.PostAsync(query);
      var results = resp.data.search;
      // Luke SkywalkER, Darth VadER, ImpERial shuttle
      Assert.AreEqual(3, results.Count, "expected 3 objects");
    }

  }
}
