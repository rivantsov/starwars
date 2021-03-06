﻿using System;
using System.Collections.Generic;
using System.Text;
using NGraphQL.CodeFirst;

namespace StarWars.Api {

  /// <summary>A starship. </summary>
  [GraphQLName("Starship")]
  public class StarshipType {
    /// <summary>The ID of the starship </summary>
    [Scalar("ID")]
    public string Id { get; set; }

    /// <summary>The name of the starship </summary>
    public string Name { get; set; }

    /// <summary>Length of the starship, along the longest axis </summary>
    [GraphQLName("length")]
    public float? GetLength(LengthUnit unit = LengthUnit.Meter) { return default; }

    /// <summary> </summary>
    [Null] public float[][] Coordinates;
  }

}
