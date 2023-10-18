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
    public string Distance { get; set; }
    public int SelectedCustomerId { get; set; }

    public Inputs()
    { 
        VehicleInput = new DefaultVehicle();  
        CustomerInput = new DefaultPerson();  
    }

}
