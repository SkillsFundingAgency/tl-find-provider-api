using System.Reflection;
using Dapper;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Data;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class EmailTemplateRepositoryTests
{
    private readonly EmailTemplate _testEmailTemplate
        = new EmailTemplateBuilder()
            .BuildList()
            .First();

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(EmailTemplateRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetEmailTemplate_By_Id_Returns_Expected_Result()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>())
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory
                );

        var emailTemplate = (await repository
            .GetEmailTemplate(_testEmailTemplate.Name));

        emailTemplate.TemplateId.Should().Be(_testEmailTemplate.TemplateId);
        emailTemplate.Name.Should().Be(_testEmailTemplate.Name);
    }

    [Fact]
    public async Task GetEmailTemplate_By_Id_Calls_Query()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>())
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository
            .GetEmailTemplate(_testEmailTemplate.TemplateId);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Is<string>(sql =>
                    !string.IsNullOrEmpty(sql)
                    && sql.Contains("SELECT TOP(1) TemplateId, Name")
                    && sql.Contains("FROM dbo.EmailTemplate")
                    && sql.Contains("WHERE TemplateId = @templateId")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters)
                );
    }

    [Fact]
    public async Task GetEmailTemplate_By_Id_Sets_Dynamic_Parameter()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>()
                )
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository
            .GetEmailTemplate(_testEmailTemplate.TemplateId);
        
        var fieldInfo = dynamicParametersWrapper.DynamicParameters.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(p => p.Name == "templates");

        fieldInfo.Should().NotBeNull();
        var templates = fieldInfo!.GetValue(dynamicParametersWrapper.DynamicParameters) as IList<object>;
        templates.Should().NotBeNullOrEmpty();

        var item = templates!.First();
        var pi = item.GetType().GetProperties();
        pi.Length.Should().Be(1);

        var dynamicProperty = pi.Single();
        dynamicProperty.Name.Should().Be("templateId");
        dynamicProperty.GetValue(item).Should().Be(_testEmailTemplate.TemplateId);
    }

    [Fact]
    public async Task GetEmailTemplate_By_Name_Returns_Expected_Result()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>())
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory
                );

        var emailTemplate = (await repository
            .GetEmailTemplateByName(_testEmailTemplate.Name));

        emailTemplate.TemplateId.Should().Be(_testEmailTemplate.TemplateId);
        emailTemplate.Name.Should().Be(_testEmailTemplate.Name);
    }

    [Fact]
    public async Task GetEmailTemplate_By_Name_Calls_Query()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>())
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository
            .GetEmailTemplateByName(_testEmailTemplate.Name);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Is<string>(sql =>
                    !string.IsNullOrEmpty(sql)                    && sql.Contains("SELECT TOP(1) TemplateId, Name")
                    && sql.Contains("FROM dbo.EmailTemplate")
                    && sql.Contains("WHERE Name = @templateName")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters)
                );
    }

    [Fact]
    public async Task GetEmailTemplate_By_Name_Sets_Dynamic_Parameter()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmailTemplate>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>()
                )
            .Returns(new List<EmailTemplate> { _testEmailTemplate });

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new EmailTemplateRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository
            .GetEmailTemplateByName(_testEmailTemplate.Name);

        var fieldInfo = dynamicParametersWrapper.DynamicParameters.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(p => p.Name == "templates");

        fieldInfo.Should().NotBeNull();
        var templates = fieldInfo!.GetValue(dynamicParametersWrapper.DynamicParameters) as IList<object>;
        templates.Should().NotBeNullOrEmpty();

        var item = templates!.First();
        var pi = item.GetType().GetProperties();
        pi.Length.Should().Be(1);

        var dynamicProperty = pi.Single();
        dynamicProperty.Name.Should().Be("templateName");
        dynamicProperty.GetValue(item).Should().Be(_testEmailTemplate.Name);
    }
}