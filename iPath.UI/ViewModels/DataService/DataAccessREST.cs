using iPath.Application.Features;
using MediatR;

namespace iPath.UI.ViewModels.DataService;

public class DataAccessREST : IDataAccess
{
    private readonly HttpClient _http;

    public DataAccessREST(HttpClient http)
    {
       _http = http;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            var resp = (TResponse)Activator.CreateInstance(typeof(TResponse), false, ex.InnerException?.Message ?? ex.Message, null);
            return resp;
        }
    }
}
