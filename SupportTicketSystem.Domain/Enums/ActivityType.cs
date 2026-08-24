namespace SupportTicketSystem.Domain.Enums;

public enum ActivityType
{
    Created = 1,
    //StatusChanged = 2,
    PriorityChanged = 3,            // not to display to customer
    Assigned = 4,
    //Unassigned = 5,
    //CommentAdded = 6,               // not to display
    //TimeLogged = 7,                 // not to display
    Resolved = 8,
    Closed = 9,
    ActivityStatusChanged = 10      // not to display to customer
}
