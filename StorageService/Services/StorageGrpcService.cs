using Grpc.Core;
using StorageService.Grpc;

namespace StorageService.Services;

public class StorageGrpcService(IConfiguration configuration) : StorageService.Grpc.StorageService.StorageServiceBase
{
    private static readonly Dictionary<string, string> FileStore = new();
    private const string DefaultSecretKey = "storage-service-secret-2024";

    public override Task<GenerateUrlResponse> GenerateUrl(GenerateUrlRequest request, ServerCallContext context)
    {
        var validSecretKey = configuration["STORAGE_SECRET_KEY"] ?? DefaultSecretKey;
        
        // Validate secret key
        if (request.SecretKey != validSecretKey)
        {
            return Task.FromResult(new GenerateUrlResponse
            {
                Success = false,
                ErrorMessage = "Invalid secret key"
            });
        }

        var fileId = Guid.NewGuid().ToString();
        var signature = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{fileId}:{request.Name}:{request.MimeType}"));
        
        var uploadUrl = $"http://localhost:5001/api/storage/upload/{fileId}?signature={signature}";

        var response = new GenerateUrlResponse
        {
            FileId = fileId,
            UploadUrl = uploadUrl,
            Signature = signature,
            Success = true
        };

        return Task.FromResult(response);
    }

    public override Task<ValidateFileResponse> ValidateFile(ValidateFileRequest request, ServerCallContext context)
    {
        var validSecretKey = configuration["STORAGE_SECRET_KEY"] ?? DefaultSecretKey;
        
        // Validate secret key
        if (request.SecretKey != validSecretKey)
        {
            return Task.FromResult(new ValidateFileResponse
            {
                Success = false,
                ErrorMessage = "Invalid secret key"
            });
        }

        var exists = FileStore.ContainsKey(request.FileId);
        
        var response = new ValidateFileResponse
        {
            Exists = exists,
            Success = true
        };

        return Task.FromResult(response);
    }

    // Method to store uploaded files (called from HTTP controller)
    public static void StoreFile(string fileId, string fileData)
    {
        FileStore[fileId] = fileData;
    }
}