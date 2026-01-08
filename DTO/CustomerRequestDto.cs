namespace OMS_Backend.DTO
{
    public record CustomerRequest(
         string CustomerCode,
         string Name,
         string? Email,
         string? Phone,
         string? Address
     );
}
