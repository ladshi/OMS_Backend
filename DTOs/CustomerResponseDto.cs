namespace OMS_Backend.DTOs
{
    public record CustomerResponse(
        int Id,
        string CustomerCode,
        string Name,
        string? Email,
        string? Phone,
        string? Address,
        bool IsActive,
        DateTime CreatedAt
    );
}
