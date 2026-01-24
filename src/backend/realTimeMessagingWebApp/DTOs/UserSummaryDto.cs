namespace realTimeMessagingWebApp.DTOs;

public record UserSummaryDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public DateTime SignUpDate { get; init; } // dont really think this is needed but aight

    //public int PasswordLength { get; set; } // experimental feature for user profile management, if you wanna add change password feature
}
