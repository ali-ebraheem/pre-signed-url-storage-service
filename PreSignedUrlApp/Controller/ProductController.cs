using Microsoft.AspNetCore.Mvc;
using StorageService.Grpc;

namespace PreSignedUrlApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
    StorageService.Grpc.StorageService.StorageServiceClient grpcClient,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("get-upload-url")]
    public async Task<IActionResult> GetUploadUrl([FromBody] FileMetadata metadata)
    {
        var secretKey = configuration["StorageService:SecretKey"];

        var request = new GenerateUrlRequest
        {
            SecretKey = secretKey,
            Name = metadata.Name,
            MimeType = metadata.MimeType,
            Size = metadata.Size
        };

        var response = await grpcClient.GenerateUrlAsync(request);

        if (!response.Success)
        {
            return BadRequest(new { error = response.ErrorMessage });
        }

        return Ok(new
        {
            fileId = response.FileId,
            uploadUrl = response.UploadUrl,
            signature = response.Signature
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
    {
        var secretKey = configuration["StorageService:SecretKey"];

        var validateRequest = new ValidateFileRequest
        {
            SecretKey = secretKey,
            FileId = request.ImageId
        };

        var response = await grpcClient.ValidateFileAsync(validateRequest);

        if (!response.Success)
        {
            return BadRequest(new { error = response.ErrorMessage });
        }

        if (!response.Exists)
            return BadRequest("Invalid image");

        // Normally insert into DB, simplified here
        return Ok(new { success = true, productId = Guid.NewGuid(), name = request.Name, imageId = request.ImageId });
    }
}

public class ProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string ImageId { get; set; } = string.Empty;
}

public class FileMetadata
{
    public string Name { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
}