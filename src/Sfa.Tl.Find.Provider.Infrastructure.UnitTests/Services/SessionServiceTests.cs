using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Infrastructure.Services;
using Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Services;
public class SessionServiceTests
{
    private const string TestEnvironment = "TEST";
    private const string TestKey = "key";
    private const string TestEnvironmentWithKey = $"{TestEnvironment}_{TestKey}";
    private const string TestValue = "value";

    private class TestValueWrapper
    {
        public string? Value { get; init; }
    }

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(SessionService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(SessionService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public void Clear_Successfully_Removes_Keys()
    {
        var session = Substitute.For<ISession>();
        var keys = new List<string> { TestEnvironmentWithKey };

        session
            .When(x => x.Clear())
            .Do(_ =>
            {
                    keys.Clear();
            });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        service.Clear();

        keys.Should().BeEmpty();
    }

    [Fact]
    public void Exists_Returns_True_If_Key_Is_Present()
    {
        var session = Substitute.For<ISession>();
        session.Keys.Returns(new List<string> { TestEnvironmentWithKey });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        var result = service.Exists(TestKey);

        result.Should().BeTrue();
    }

    [Fact]
    public void Exists_Returns_False_If_Key_Is_Not_Present()
    {
        var session = Substitute.For<ISession>();
        session.Keys.Returns(new List<string>());

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        var result = service.Exists(TestKey);

        result.Should().BeFalse();
    }

    [Fact]
    public void Get_Returns_Value_If_Key_Is_Present()
    {
        var session = Substitute.For<ISession>();
        session.Keys.Returns(new List<string> { TestEnvironmentWithKey });

        var testItem = new TestValueWrapper { Value = TestValue };
        var serializedValue = JsonSerializer.Serialize(testItem);

        session.TryGetValue(TestEnvironmentWithKey, out Arg.Any<byte[]?>())
            .Returns(x =>
            {
                x[1] = Encoding.UTF8.GetBytes(serializedValue);
                return true;
            });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        var result = service.Get<TestValueWrapper>(TestKey);

        result.Should().NotBeNull();
        result!.Value.Should().Be(TestValue);
    }

    [Fact]
    public void Get_Returns_Null_If_Key_Not_Present()
    {
        var session = Substitute.For<ISession>();
        session.Keys.Returns(new List<string>());
        session.TryGetValue(Arg.Any<string>(), out _)
            .Returns(false);

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        var result = service.Get<string>(TestKey);

        result.Should().BeNull();
    }

    [Fact]
    public void Remove_Successfully_Removes_Key_If_Present()
    {
        var session = Substitute.For<ISession>();
        var keys = new List<string> { TestEnvironmentWithKey };

        session
            .When(x => x.Remove(TestEnvironmentWithKey))
            .Do(x =>
            {
                var k = x[0]?.ToString();
                var idx = k != null ? keys.IndexOf(k) : -1;
                if (idx > -1)
                {
                    keys.RemoveAt(idx);
                }
            });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        service.Remove(TestKey);

        keys.Should().NotContain(TestEnvironmentWithKey);
    }

    [Fact]
    public void Remove_Does_Not_Removes_Key_If_Not_Present()
    {
        var session = Substitute.For<ISession>();
        var keys = new List<string> { TestEnvironmentWithKey };

        session
            .When(x => x.Remove(TestEnvironmentWithKey))
            .Do(x =>
            {
                var k = x[0]?.ToString();
                var idx = k != null ? keys.IndexOf(k) : -1;
                if (idx > -1)
                {
                    keys.RemoveAt(idx);
                }
            });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var service = new SessionServiceBuilder().Build(contextAccessor);

        service.Remove("Not_A_Valid_Key");

        keys.Should().Contain(TestEnvironmentWithKey);
    }

    [Fact]
    public void Set_Successfully_Saves_Key_And_Value()
    {
        var session = Substitute.For<ISession>();
        var dictionary = new Dictionary<string, byte[]?>();

        session
            .When(x => x.Set(TestEnvironmentWithKey, Arg.Any<byte[]>()))
            .Do(x =>
            {
                var k = x[0]?.ToString();
                if (k != null)
                {
                    var v = x[1] as byte[];
                    dictionary[k] = v;
                }
            });

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        contextAccessor.HttpContext.Returns(new DefaultHttpContext
        {
            Session = session
        });

        var testItem = new TestValueWrapper { Value = TestValue };

        var service = new SessionServiceBuilder().Build(contextAccessor);

        service.Set(TestKey, testItem);

        dictionary.Should().ContainKey(TestEnvironmentWithKey);
        dictionary[TestEnvironmentWithKey].Should().NotBeNull();
        var stringValue = Encoding.UTF8.GetString(dictionary[TestEnvironmentWithKey]!);
        var rehydratedValue = JsonSerializer.Deserialize<TestValueWrapper>(stringValue);
        rehydratedValue.Should().NotBeNull();
        rehydratedValue.Should().BeEquivalentTo(testItem);
    }
}
