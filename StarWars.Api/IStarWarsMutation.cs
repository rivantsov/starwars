﻿using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  public interface IStarWarsMutation {
    Review_ CreateReview(Episode episode, ReviewInput_ reviewInput);
  }
}
