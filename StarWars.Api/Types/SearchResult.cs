using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public class SearchResult : Union<HumanType, DroidType, StarshipType> { }

}
