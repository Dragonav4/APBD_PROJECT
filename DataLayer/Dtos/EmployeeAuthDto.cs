namespace APBD_PROJECT.DataLayer.Dtos;

public class EmployeeAuthDto
{
    public record EmployeeLoginDto(
        string Username, 
        string Password);
    public record EmployeeResponseDto(
        long Id, 
        string Username, 
        string Role);
}