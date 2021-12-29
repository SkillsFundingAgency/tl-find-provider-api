using System;
using AutoFixture.Idioms;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public class ArgumentBehaviorException : IBehaviorExpectation
{
    public void Verify(IGuardClauseCommand command)
    {
        try
        {
            command.Execute(null);
        }
        catch (ArgumentNullException)
        {
            return;
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new GuardClauseException();
    }
}