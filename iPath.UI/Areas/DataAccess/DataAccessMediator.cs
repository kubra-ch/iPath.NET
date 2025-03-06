using iPath.Application.Features;
using MediatR;
using Org.BouncyCastle.Tsp;

namespace iPath.UI.Areas.DataAccess;

public class DataAccessMediator(IMediator mediator, ILogger<DataAccessMediator> logger) : IDataAccess
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) where TResponse : BaseResponse
    {
        try
        {
            return await mediator.Send(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);

            if(typeof(TResponse).IsSubclassOf(typeof(BaseResponse)))
            {
                try
                {
                    var resp = (TResponse)Activator.CreateInstance(typeof(TResponse), false, ex.InnerException?.Message ?? ex.Message, null);
                    return resp;
                }
                catch(Exception ex2)
                {
                    logger.LogWarning(ex2.Message);
                }
            }

            return (TResponse) new BaseResponse(false, ex.Message);
        }
    }
}
