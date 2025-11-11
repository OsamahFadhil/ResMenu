using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;

namespace Menufy.Application.Features.Files.Commands.UploadImage;

public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<string>>
{
    private readonly IFileStorageService _fileStorageService;

    public UploadImageCommandHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<string>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(request.ContentType.ToLower()))
        {
            return Result<string>.FailureResult("Invalid file type. Only JPEG, PNG, WEBP, and GIF images are allowed.");
        }

        // Validate file size (max 5MB)
        if (request.FileStream.Length > 5 * 1024 * 1024)
        {
            return Result<string>.FailureResult("File size exceeds 5MB limit.");
        }

        try
        {
            var fileUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken
            );

            return Result<string>.SuccessResult(fileUrl);
        }
        catch (Exception ex)
        {
            return Result<string>.FailureResult($"Failed to upload image: {ex.Message}");
        }
    }
}
