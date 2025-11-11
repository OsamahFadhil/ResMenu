using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Files.Commands.UploadImage;

public class UploadImageCommand : IRequest<Result<string>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
