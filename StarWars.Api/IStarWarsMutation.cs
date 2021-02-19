using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public interface IStarWarsMutation {
    ReviewType CreateReview(Episode episode, ReviewInput reviewInput);
  }
}
