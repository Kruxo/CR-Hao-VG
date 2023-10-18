using Car_Rental.Common.Interfaces;
using Car_Rental.Common.InputsClasses;

namespace Car_Rental.Business.Classes;

public class Inputs
{
    public IVehicle VehicleInput { get; set; }
    public IPerson CustomerInput { get; set; }
    public bool Delay { get; set; }
    public bool Processing { get; set; }
    public string Message { get; set; }
    public string Distance { get; set; } //Används för att nollställa input fields för distance
    public int SelectedCustomerId { get; set; } //Används för html och felhantering när kund inte är vald 

    public Inputs()
    { 
        VehicleInput = new DefaultVehicle();  
        CustomerInput = new DefaultPerson();  
    }

}
