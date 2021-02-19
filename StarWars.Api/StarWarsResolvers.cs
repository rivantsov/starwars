using System;
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
      _app = (StarWarsApp) request.App;
    }

    public void EndRequest(IRequestContext request) {
    }

    public Episode[] GetEpisodes(IFieldContext fieldContext) {
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

    // the field expects type  ICharacter_[] here; but we cannot create instances of ICharacter_
    //  at runtime. Human_ and Droid_ implement it but we cannot create them, we must return 
    //  entities (Human or Droid) from resolvers. So for interface return type the engine accepts
    //  ANY return type (including Object) and will enforce type check at runtime
    //  We use Character class here, it is pure application-layer type, not registered with API,
    //  because it is convenient; the engine does not care. 
    public IList<Character> GetCharacters(IFieldContext fieldContext, Episode episode) { 
      return _app.GetCharacters(episode); 
    }

    // see comment to GetCharacters. We could return Character but use Object type, as a demo
    public object GetCharacter(IFieldContext fieldContext, string id) {
      return _app.GetCharacter(id);
    }

    public IList<Review> GetReviews(IFieldContext fieldContext, Episode episode) { 
      return _app.GetReviews(episode); 
    }

    public float? GetHeight(IFieldContext fieldContext, Human human, LengthUnit unit) {
      if (human.Height == null)
        return null;
      else if (unit == LengthUnit.Meter)
        return human.Height;
      else
        return human.Height * 3.28f; // feet
    }

    public float? GetLength(IFieldContext fieldContext, Starship starship, LengthUnit unit) {
      if (starship.Length == null)
        return null;
      else if (unit == LengthUnit.Meter)
        return starship.Length;
      else 
        return starship.Length * 3.28f; // feet
    }

    // We return Union instances here, we wrap raw result objects into SearchResult_ objects, 
    //  but we could also return objects (see SearchSimple). So return type may be IList<object>
    public IList<SearchResult> Search(IFieldContext fieldContext, string text) { 
      var results = _app.Search(text)
                        .Select(r => new SearchResult() { Value = r })
                        .ToList();
      return results;
    }

    // this works too
    public IList<object> SearchSimple(IFieldContext fieldContext, string text) {
      return _app.Search(text).ToList();
    }

    // Mutations
    public Review CreateReview(IFieldContext fieldContext, Episode episode, ReviewInput reviewInput) {
      return _app.CreateReview(episode, reviewInput.Stars, reviewInput.Commentary, reviewInput.Emojis); 
    }

    // batched version of GetStarships
    public IList<Starship> GetStarshipsBatched(IFieldContext fieldContext, Human human) {
      // batch execution (aka DataLoader); we retrieve all pending parents (humans),
      //  get all their starships to a dictionary, and then post it back into context - 
      //  the engine will use this dictionary to lookup values and will not call resolver anymore
      var allParents = fieldContext.GetAllParentEntities<Human>();
      var shipsByHuman = allParents.ToDictionary(h => h, h => h.Starships); //  _app.GetFriendLists(allParents);
      fieldContext.SetBatchedResults<Human, IList<Starship>>(shipsByHuman, new List<Starship>());
      return null; // the engine will use batch results dict to lookup the value
    }
    
  }
}
