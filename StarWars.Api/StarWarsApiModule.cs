using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public class StarWarsApiModule: GraphQLModule {
    public StarWarsApiModule() {
      // Register types
      this.EnumTypes.Add(typeof(Episode), typeof(LengthUnit), typeof(Emojis));
      this.ObjectTypes.Add(typeof(HumanType), typeof(DroidType), typeof(StarshipType), typeof(ReviewType));
      this.InterfaceTypes.Add(typeof(ICharacter));
      this.UnionTypes.Add(typeof(SearchResult));
      this.InputTypes.Add(typeof(ReviewInput));
      this.QueryType = typeof(IStarWarsQuery);
      this.MutationType = typeof(IStarWarsMutation);

      // Register resolver class
      this.ResolverTypes.Add(typeof(StarWarsResolvers));

      // map app entity types to GraphQL Api types
      MapEntity<Human>().To<HumanType>(h => new HumanType() {
        Mass = h.MassKg, // we use custom mapping expression here, others with matching names are mapped automatically
      });
      MapEntity<Droid>().To<DroidType>();
      MapEntity<Starship>().To<StarshipType>();
      MapEntity<Review>().To<ReviewType>();
      
    } //constructor  

  }
}
