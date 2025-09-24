using System.Text;
using Microsoft.AspNetCore.Mvc;
using StorageService.Services;

namespace StorageService.Controller;

[ApiController]
[Route("api/storage")]
public class StorageController : ControllerBase
{
    [HttpPost("upload/{fileId}")]
    public async Task<IActionResult> UploadFile([FromForm] UploadRequest request, string fileId,
        [FromQuery] string signature)
    {
        // Console.WriteLine(request.File.FileName);
        // Console.WriteLine(request.File.ContentType);
        // Console.WriteLine(request.File.Length);
 
        // Validate signature before processing the file
        if (!ValidateSignature(fileId, request.File.FileName, request.File.ContentType, signature))
        {
            return Unauthorized("Invalid signature");
        }

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);
        
        // Store file using the gRPC service's static method
        StorageGrpcService.StoreFile(fileId, Convert.ToBase64String(ms.ToArray()));

        return Ok(new { fileId });
    }

    private static bool ValidateSignature(string fileId, string fileName, string mimeType, string providedSignature)
    {
        try
        {
            var expectedSignatureData = $"{fileId}:{fileName}:{mimeType}";
            var expectedSignature = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(expectedSignatureData));

            return expectedSignature == providedSignature;
        }
        catch
        {
            return false;
        }
    }
}

public record UploadRequest(IFormFile File);