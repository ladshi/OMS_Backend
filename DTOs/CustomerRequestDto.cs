namespace OMS_Backend.DTOs
{
    public record CustomerRequest(
         string CustomerCode,
         string Name,
         string? Email,
         string? Phone,
         string? Address
     );
}
