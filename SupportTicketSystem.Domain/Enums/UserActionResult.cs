namespace SupportTicketSystem.Domain.Enums;
public enum UserActionResult
{
    Success,
    NotFound,
    Failed,
    InvalidUserType,
    EmailAlreadyExists,
    InvalidPassword,
    Unauthorized
}