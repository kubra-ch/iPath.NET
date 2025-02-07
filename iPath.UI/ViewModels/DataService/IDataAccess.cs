using iPath.Application.Features;
using MediatR;

namespace iPath.UI.ViewModels.DataService;

public interface IDataAccess
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
