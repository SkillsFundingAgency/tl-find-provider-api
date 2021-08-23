using System;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime MinValue => DateTime.MinValue;
        public DateTime MaxValue => DateTime.MaxValue;
        public DateTime Today => DateTime.Today;
    }
}
