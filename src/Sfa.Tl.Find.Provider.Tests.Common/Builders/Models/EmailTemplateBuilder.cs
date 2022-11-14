using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmailTemplateBuilder
{
    public IEnumerable<EmailTemplate> BuildList()
    {
        return new List<EmailTemplate>
        {
            new()
            {
                TemplateId = "a229b7b3-705a-486f-baea-304a3f1521a4",
                Name = "TestTemplateOne"
            },
            new()
            {
                TemplateId = "21c673d3-f86f-410b-85e6-5a873097e5c6",
                Name = "TestTemplateTwo"
            },
            new()
            {
                TemplateId = "b15f9db7-031c-49c3-a1e0-89e22b130954",
                Name = "TestTemplateThree"
            }
        };
    }
}