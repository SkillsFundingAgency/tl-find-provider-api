using System;

namespace Sfa.Tl.Find.Provider.Api.Models.Exceptions;

public class PostcodeNotFoundException : Exception
{
    public string Postcode { get; }

    public PostcodeNotFoundException(string postcode, Exception innerException = null) 
        : base($"Postcode {postcode} was not found", innerException)
    {
        Postcode = postcode;
    }
}