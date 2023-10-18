using Car_Rental.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental.Common.InputsClasses;

public class DefaultPerson : IPerson
{
    public int Id { get; set; }
    public string SocialSecurityNumber { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
}