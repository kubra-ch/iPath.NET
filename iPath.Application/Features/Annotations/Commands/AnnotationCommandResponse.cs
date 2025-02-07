using iPath.Data.Entities;

namespace iPath.Application.Features;

public record AnnotationCommandResponse(bool Sucess, string? Message = null, Annotation? Data = null)
    : BaseResponseT<Annotation>(Sucess, Message, Data);
