using iPath.Application.Features;
using MediatR;

namespace iPath.UI.Areas.DataAccess;

public interface IDataAccess
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) where TResponse : BaseResponse;
}

