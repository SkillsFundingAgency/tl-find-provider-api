﻿using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(OrganisationName) + ", nq}")]
public class EmployerInterestSummaryDto
{
    public int Id { get; init; }
    public string OrganisationName { get; init; }
    public string Industry { get; init; }
    public string Postcode { get; init; }
    public double? Distance { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ModifiedOn { get; init; }
}
