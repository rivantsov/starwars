﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public class StarWarsResolvers : IResolverClass {
    StarWarsApp _app;

    // Begin/end request method
    public void BeginRequest(IRequestContext request) {
      // Get app instance
      var swApi = (StarWarsApi)request.Server.Api;
      _app = swApi.App;
    }

    public void EndRequest(IRequestContext request) {
    }

    public Episode GetEpisodes(IFieldContext fieldContext) {
      return _app.GetAllEpisodes();
    }

    public IList<Starship> GetStarships(IFieldContext fieldContext) {
      return _app.GetStarships();
    }

    // example of async method
    public async Task<Starship> GetStarshipAsync(IFieldContext fieldContext, string id) {
      await Task.Delay(10); 
      return _app.GetStarship(id);
    }

    public IList<Character> GetCharacters(IFieldContext fieldContext, Episode episode) { 
      return _app.GetCharacters(episode); 
    }

    public Character GetCharacter(IFieldContext fieldContext, string id) {
      return _app.GetCharacter(id);
    }

    public IList<Review> GetReviews(IFieldContext fieldContext, Episode episode) { 
      return _app.GetReviews(episode); 
    }

    public float? GetHeight(IFieldContext fieldContext, Human human, LengthUnit unit = LengthUnit.Meter) { 
      if (unit == LengthUnit.Foot)
        return human.Height * 3.28f; 
      else
        return human.Height;
    }

    public float? GetLength(IFieldContext fieldContext, Starship starship, LengthUnit unit = LengthUnit.Meter) {
      if (unit == LengthUnit.Foot)
        return starship.Length * 3.28f;
      else
        return starship.Length;
    }

    // We return Union instances here, we wrap raw result objects into SearchResult_ objects, 
    //  but we could also return objects (see SearchSimple). So return type may be IList<object>
    public IList<SearchResult_> Search(IFieldContext fieldContext, string text) { 
      var results = _app.Search(text)
                        .Select(r => new SearchResult_() { Value = r })
                        .ToList();
      return results;
    }

    // this works too
    public IList<object> SearchSimple(IFieldContext fieldContext, string text) {
      return _app.Search(text).ToList();
    }

    // Mutations
    public Review CreateReview(IFieldContext fieldContext, Episode episode, ReviewInput_ reviewInput) {
      return _app.CreateReview(episode, reviewInput.Stars, reviewInput.Commentary, reviewInput.Emojis); 
    }

    // this is a default, non-batched version, not used - we use batched version instead
    public IList<Character> GetFriends(IFieldContext fieldContext, Character character) {
      return character.Friends;
    }

    // batched version
    public IList<Character> GetFriendsBatched(IFieldContext fieldContext, Character character) {
      // batch execution (aka DataLoader); we retrieve all pending parents (characters)
      //  get all their friend lists as a dictionary, and then post it into context - 
      //  the engine will use this dictionary to lookup values and will not call resolver anymore
      var allParents = fieldContext.GetAllParentEntities<Character>();
      var friendsByCharacter =  _app.GetFriendLists(allParents);
      fieldContext.SetBatchedResults<Character, IList<Character>>(friendsByCharacter); 
      return null; // the engine will use batch results dict to lookup the value
    }
    
  }
}
