namespace SupportTicketSystem.Domain.Enums;

public enum ActivityType
{
    Created = 1,
    StatusChanged = 2,
    PriorityChanged = 3,
    Assigned = 4,
    Unassigned = 5,
    CommentAdded = 6,
    TimeLogged = 7,
    Resolved = 8,
    Closed = 9
}
