using iPath.Data.Entities;

namespace iPath.Application.Services.Storage;

public interface IStorageService
{
    string StoragePath { get; }

    Task<StorageRepsonse> PutNodeFileAsync(int NodeId, CancellationToken ctk = default!);
    Task<StorageRepsonse> GetNodeFileAsync(int NodeId, CancellationToken ctk = default!);


    Task<StorageRepsonse> PutNodeJsonAsync(int NodeId, CancellationToken ctk = default!);
    Task<StorageRepsonse> PutNodeJsonAsync(Node node, CancellationToken ctk = default!);
}


public record StorageRepsonse(bool Success, string? StorageId = null, string? Message = null!);