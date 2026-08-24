namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketGridResponse
{
    public IReadOnlyList<TicketGridItemResponse> Items { get; set; }
        = new List<TicketGridItemResponse>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool IsCustomer { get; set; } 

    public bool IsAgent { get; set; }

    public List<string> SortingColumns { get; set; } = new List<string>();
}