using Sfa.Tl.Find.Provider.Application.Models.Exceptions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Exceptions;

public class PostcodeNotFoundExceptionTests
{
    [Fact]
    public void PostcodeNotFoundException_Sets_Expected_Message()
    {
        const string postcode = "A1A B2B";
        const string expectedMessage = $"Postcode {postcode} was not found";

        var exception = new PostcodeNotFoundException(postcode);

        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().BeNull();
        exception.Postcode.Should().Be(postcode);
    }

    [Fact]
    public void PostcodeNotFoundException_Sets_Expected_Message_And_Inner_Exception()
    {
        const string postcode = "A1A B2B";
        const string expectedMessage = $"Postcode {postcode} was not found";
        var innerException = new Exception("Test inner exception");

        var exception = new PostcodeNotFoundException(postcode, innerException);

        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().Be(innerException);
        exception.Postcode.Should().Be(postcode);
    }
}