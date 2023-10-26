﻿using System;
using System.Collections.Generic;

namespace NZWalks.API.Models.Domain;

public class Region
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string? RegionImageUrl { get; set; }

    //public virtual ICollection<Walk> Walks { get; set; } = new List<Walk>();
}
