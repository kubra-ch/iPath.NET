using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.Application.Features;

public record BaseResponse
{
    public bool Success { get; }
    public string? Message { get; }

    public BaseResponse(bool Sucess, string? Message = null)
    {
        Success = Sucess;
        this.Message = Message;
    }
};



public record BaseResponseT<T> : BaseResponse where T : class
{
    public T? Data { get; }

    public BaseResponseT(bool Success, string? Message = null, T? Data = null) : base(Success, Message)
    {
        this.Data = Data;
    }
}


public record ErrorResponse(string Message) : BaseResponse(false, Message);

public record SuccessResponse() : BaseResponse(true);





public class BaseRequest<TResponse> : IRequest<TResponse> where TResponse : BaseResponse { }