namespace APBD_PROJECT.DataLayer.Dtos;

public class PersonalClientDto
{
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string Email { get; set; }
   public string Phone { get; set; }
   public string Address { get; set; }
   public string Pesel {get; set; }
}

public class CompanyClientDto
{
    public string CompanyName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Krs {get; set; }
}

public class ClientDto
{
    public long Id { get; set; }
    public PersonalClientDto? Person { get; set; }
    public CompanyClientDto? Company { get; set; }
}