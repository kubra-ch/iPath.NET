using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace iPath.Application.Features;

public record BaseResponse
{
    public bool Success { get; }
    public string? Message { get; }

    public BaseResponse(bool Sucess, string? Message = null)
    {
        this.Success = Sucess;
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
