using System;

namespace Sfa.Tl.Find.Provider.Api.Models.Exceptions
{
    public class PostcodeNotFoundException : Exception
    {
        public PostcodeNotFoundException(string postcode)
          : base ($"Postcode {postcode} was not found")
        {
        }

        public PostcodeNotFoundException(string postcode, Exception innerException) 
            : base($"Postcode {postcode} was not found", innerException)
        {
        }
    }
}
