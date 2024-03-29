﻿using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYear
{
    public short Year { get; init; }

    public ICollection<Qualification> Qualifications { get; init; } = new List<Qualification>();

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Qualifications != null ? Qualifications.Count : "null")} Qualifications";
}