using Menufy.Application.Common.Interfaces;

namespace Menufy.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;

    public FileStorageService(string uploadPath = "uploads", string baseUrl = "http://localhost:5000")
    {
        _uploadPath = uploadPath;
        _baseUrl = baseUrl;

        // Create uploads directory if it doesn't exist
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique filename
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            // Save file
            using (var fileStreamOutput = File.Create(filePath))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }

            return GetFileUrl(uniqueFileName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to upload file: {ex.Message}", ex);
        }
    }

    public Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_baseUrl}/uploads/{fileName}";
    }
}
