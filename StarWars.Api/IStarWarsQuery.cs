using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  interface IStarWarsQuery {

    /// <summary>List of episodes.</summary>
    [Resolver("GetEpisodes")]
    Episode[] Episodes { get; }

    /// <summary>List of all starships.</summary>
    [Resolver("GetStarships")]
    IList<Starship_> Starships { get; }

    [GraphQLName("starship"), Resolver(nameof(StarWarsResolvers.GetStarshipAsync))]
    Starship_ GetStarship([Scalar("ID")] string id);

    [GraphQLName("characters")]
    IList<ICharacter_> GetCharacters(Episode episode);

    [GraphQLName("character")]
    ICharacter_ GetCharacter([Scalar("ID")] string id);

    [GraphQLName("reviews")]
    IList<Review_> GetReviews(Episode episode);

    [Resolver(nameof(StarWarsResolvers.Search))]
    IList<SearchResult_> Search(string text);
  }
}
