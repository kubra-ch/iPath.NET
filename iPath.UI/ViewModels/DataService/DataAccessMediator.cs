using iPath.Application.Features;
using MediatR;

namespace iPath.UI.ViewModels.DataService;

public class DataAccessMediator(IMediator mediator, ILogger<DataAccessREST> logger) : IDataAccess
{

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await mediator.Send(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            var resp = (TResponse)Activator.CreateInstance(typeof(TResponse), false, ex.InnerException?.Message ?? ex.Message);
            return resp;
        }
    }
}
