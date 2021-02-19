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
    IList<StarshipType> Starships { get; }

    [GraphQLName("starship"), Resolver(nameof(StarWarsResolvers.GetStarshipAsync))]
    StarshipType GetStarship([Scalar("ID")] string id);

    [GraphQLName("characters")]
    IList<ICharacter> GetCharacters(Episode episode);

    [GraphQLName("character")]
    ICharacter GetCharacter([Scalar("ID")] string id);

    [GraphQLName("reviews")]
    IList<ReviewType> GetReviews(Episode episode);

    [Resolver(nameof(StarWarsResolvers.Search))]
    IList<SearchResult> Search(string text);
  }
}
