using Sfa.Tl.Find.Provider.Infrastructure.ParameterTransformers;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.ParameterTransformers;
public class ParameterTransformersTests
{
    [Theory(DisplayName = $"{nameof(SlugifyParameterTransformer)} Data Tests")]
    [InlineData(null, null)]
    [InlineData("Index", "index")]
    [InlineData("EmployerList", "employer-list")]
    public void SlugifyParameterTransformerDataTests(object value, string expectedResult)
    {
        var transformer = new SlugifyParameterTransformer();
        var result = transformer.TransformOutbound(value);
        result.Should().Be(expectedResult);
    }
}
