﻿using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public class StarWarsApiModule: GraphQLModule {
    public StarWarsApiModule() {
      // Register types
      this.EnumTypes.AddRange(new Type[] { typeof(Episode), typeof(LengthUnit), typeof(Emojis) });
      this.ObjectTypes.AddRange(new Type[] { typeof(Human_), typeof(Droid_), typeof(Starship_), typeof(Review_) });
      this.InterfaceTypes.Add(typeof(ICharacter_));
      this.UnionTypes.Add(typeof(SearchResult_));
      this.InputTypes.Add(typeof(ReviewInput_));
      this.QueryType = typeof(IStarWarsQuery);
      this.MutationType = typeof(IStarWarsMutation);
      this.ResolverTypes.Add(typeof(StarWarsResolvers));

      // map app entity types to GraphQL Api types
      MapEntity<Human>().To<Human_>(h => new Human_() {
        Mass = h.MassKg, // we use custom mapping expression here, others with matching names are mapped automatically
      });
      MapEntity<Droid>().To<Droid_>();
      MapEntity<Starship>().To<Starship_>();
      MapEntity<Review>().To<Review_>();
      // MapEntity<Character>().To<ICharacter_>();
      // MapEntity<NamedObject>().ToUnion<SearchResult_>();
      
    } //constructor  

  }
}
