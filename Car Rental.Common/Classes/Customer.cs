using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Customer : IPerson
{
    public int Id { get; set; }
    public int SocialSecurityNumber { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }

    public Customer(int id, int socialSecurityNumber, string lastName, string firstName)
    {
        Id = id;
        SocialSecurityNumber = socialSecurityNumber;
        LastName = lastName;
        FirstName = firstName;
    }
}
