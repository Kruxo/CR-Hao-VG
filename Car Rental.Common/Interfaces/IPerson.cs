﻿namespace Car_Rental.Common.Interfaces;

public interface IPerson
{
    public int Id { get; set; }
    public string SocialSecurityNumber { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
}
