using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Terminology;
using CodeableConcept = iPath.Data.Entities.CodeableConcept;

namespace iPath.Application.SnomedCT;

public class SnomedClient(IHttpClientFactory fct)
{

    public async Task<List<CodeableConcept>> Lookup(string ValueSet, string? SearchValue)
    {
        var http = fct.CreateClient("SnomedFhir");
        var settings = new FhirClientSettings
        {
            Timeout = 0,
            PreferredFormat = ResourceFormat.Json,
            VerifyFhirVersion = true,
            ReturnPreference = ReturnPreference.Minimal
        };
        var client = new FhirClient(http.BaseAddress.ToString(), http, settings);

        var svc = new ExternalTerminologyService(client);
        var parameters = new ExpandParameters()
        .WithPaging(0, 100);

        if (!string.IsNullOrEmpty(SearchValue) && SearchValue.Length > 2)
        {
            parameters = parameters.WithFilter(SearchValue);
        }

        if (!string.IsNullOrEmpty(ValueSet))
        {
            parameters = parameters.WithValueSet(ValueSet);
        }

        var vs = await svc.Expand(parameters: parameters, useGet: true) as ValueSet;

        return vs.Expansion.Contains.Select(x => new CodeableConcept { 
            System = "http://snomed.info/sct", 
            Code = x.Code, 
            Display = x.Display
        })
            .ToList();
    }

}
