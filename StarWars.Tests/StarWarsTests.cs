using System.Collections;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using NGraphQL;
using NGraphQL.Server;
using NGraphQL.Utilities;
using StarWars.Api;
using NGraphQL.Client;

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
      ResponseData resp;


      {
        query = @" query { starships{name, length coordinates} } ";
        var respD = await TestEnv.Client.PostAsync(query);
        var ships0Name = respD.data.starships[0].name;
        Assert.IsNotNull(ships0Name, "expected name");
      }


      query = @" query { starships{name, length coordinates} } ";
      resp = await TestEnv.Client.PostAsync(query);
      var ships = resp.data.starships;
      Assert.AreEqual(4, ships.Count, "expected 4 ships");

      query = @" query { starship(id: ""3001"") {name, length} } ";
      resp = await TestEnv.Client.PostAsync(query);
      var shipName = resp.data.GetValue<string>("starship.name");
      Assert.AreEqual("X-Wing", shipName);

      // character query with friends
      query = @" {
  leia: character(id: ""1003"") { 
    name
    friends { name }
  }
}";
      resp = await TestEnv.Client.PostAsync(query);
      var lname = resp.data.leia.name;
      Assert.AreEqual("Leia Organa", lname);
      var leiaFriends = resp.data.leia.friends;
      Assert.AreEqual(4, leiaFriends.Count, "Expected 4 friends");

    }

    [TestMethod]
    public async Task TestBatching() {
      string query;
      ResponseData resp;
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
      StarWarsResolvers.CallCount_GetStarships = 0; //reset the counter
      resp = await TestEnv.Client.PostAsync(query);
      var charList = resp.data.charList;
      Assert.IsTrue(charList.Count >= 4, "at least 4 characters expected"); 
      // there are 4 humans in the list, each has 'starships' field, but there was only one call to the resolver;
      //  the resolver performed batched retrieval
      Assert.AreEqual(1, StarWarsResolvers.CallCount_GetStarships, "Expected 1 call to resolver");
    }

    [TestMethod]
    public async Task TestMutation() {
      ResponseData resp;

      // get Jedi reviews, add review, check new count
      // 1. Get Jedi reviews, get count
      var getReviewsQuery = @"
{
  reviews( episode: JEDI) { episode, stars, commentary, emojis }
}";
      resp = await TestEnv.Client.PostAsync(getReviewsQuery);
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
      ResponseData resp;

      query = @" 
query { 
  search (text: ""er"") {
    __typename,
    name, 
  } 
} ";
      resp = await TestEnv.Client.PostAsync(query);
      var results = resp.data.search;
      // Luke SkywalkER, Darth VadER, ImpERial shuttle
      Assert.AreEqual(3, results.Count, "expected 3 objects");
    }

  }
}
