using System.ComponentModel.DataAnnotations;

namespace realTimeMessagingWebApp.DTOs;

# nullable disable
public record ImageDetailsForUploadUrlDto
{

    [Required]
    public Guid UserId { get; init; }
    [Required]
    public Guid ChatId { get; init; }
    [Required]
    public string FileExtension { get; init; }
    [Required]
    public string FileType { get; init; }
}
