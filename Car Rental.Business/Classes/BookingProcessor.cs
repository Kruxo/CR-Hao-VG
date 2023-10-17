using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Exceptions;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Classes;
using Car_Rental.Data.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;

namespace Car_Rental.Business.Classes;

public class BookingProcessor
{
    private readonly IData _db;
    public BookingProcessor(IData db) => _db = db; //Pratar med våran datalager genom BookingProcessor och på så sätt få tillgång till det som finns i CollectionData, men endast efter att vi har injektserat det i Program.cs

   /* public class Inputs
    {
        public IVehicle Vehicle { get; set; }
        public IPerson Customer { get; set; }

        public bool Delay { get; set; }
        public bool Processing { get; set; }
        public string Message { get; private set; }
    } */
    

    
    //RENT & RETURN VEHICLE
    public string Make { get; set; }
    public string RegistrationNumber { get; set; }
    public double Odometer { get; set; }
    public double CostKm { get; set; }
    public string VehicleType { get; set; }
    public string Distance { get; set; }


    //CUSTOMER
    public string SSN { get; set; }
    public string LName { get; set; }
    public string FName { get; set; }
    public int SelectedCustomerId { get; set; }


    //MISCELLANEOUS
    public bool Delay { get; set; }
    public bool Processing { get; set; }
    public string Message { get; private set; }


    //GET LISTS
    public IEnumerable<IBooking> GetBookings()
    {
        return _db.Get<IBooking>(null);
    }

    public IEnumerable<Customer> GetCustomers()
    {
        return _db.Get<IPerson>(p => p is Customer).OfType<Customer>();
    }

    public IEnumerable<IVehicle> GetVehicles(VehicleStatuses status = default)
    {
        return _db.Get<IVehicle>(v => status == default || v.VStatus == status);
    }


    //GET SINGLE
    public IPerson? GetPerson(string ssn)
    {
        return _db.Single<IPerson>(p => p is Customer && (p as Customer).SocialSecurityNumber.ToString() == ssn);
    }

    public IVehicle? GetVehicle(Expression<Func<IVehicle, bool>> expression)
    {
        return _db.Single(expression);
    }

    //BOOKINGS, VEHICLES, CUSTOMERS
    public async Task<IBooking> RentVehicle(int vehicleId, int customerId)
    {
        if(SelectedCustomerId == 0) //Condition möts om ingen customer är vald och då är SelectedCustomerId = 0, vilket är default värdet.
        {
            Message = "Error! Please pick a customer.";
            return null;
        }

        Delay = true; //Boolean som används som condition när vi vill gråa ut våra knappar i html med disabled
        await Task.Delay(5000); // Simulerar att vi hämtar data från ett API med 5s fördröjning
        Delay = false;
        SelectedCustomerId = 0; //Nollställer efter lyckad renting

        return _db.RentVehicle(vehicleId, customerId);
    }

    public IBooking ReturnVehicle(int vehicleId, string distance)
    {
        if(!int.TryParse(distance, out int dist))
        {
            Message = "Error! Please enter the distance.";
            return null;
        }

        var vehicle = _db.Get<IVehicle>(v => v.Id == vehicleId).FirstOrDefault();
        var booking = _db.Get<IBooking>(b => b.VehicleBooking.RegNo == vehicle.RegNo && b.EndRent == null).FirstOrDefault();

        if (vehicle == null || booking == null || booking.KmReturned.HasValue)
        {
            return null;
        }

        booking.KmReturned = dist;

        // Status uppdateringar
        vehicle.VStatus = VehicleStatuses.Available;
        booking.EndRent = DateTime.Now;
        booking.Status = VehicleStatuses.Booked;

        //Nollställning 
        Distance = string.Empty;

        return booking;
    }

    public void AddVehicle()
    {
        try
        {
            if (string.IsNullOrEmpty(Make) || string.IsNullOrEmpty(RegistrationNumber) || string.IsNullOrEmpty(Odometer.ToString()) || string.IsNullOrEmpty(CostKm.ToString()))
            {
                throw new InputException("Error! Please fill all input fields with a value.");
            }

            if (string.IsNullOrEmpty(VehicleType))
            {
                VehicleType = VehicleTypes.Convertible.ToString(); //Default selected vtype i våran dropdown om inget är vald, programmet krascha innan då den trodde att det var null
            }

            if (_db.Get<IVehicle>(v => v is Vehicle && (v as Vehicle).RegNo == RegistrationNumber).Any()) //Any är en Linq metod som kollar om RegNo redan existerar i listan. Den returnerar en boolean beroende på om kriterierna möts eller ej.
            {                                                                                             //Ingen specifik restriktion på RegNo för exempelvis längd då man kan ha custom registreringsnummer för fordon
                Message = "Error! Registration Number already exists."; 
                return; 
            }

            var newVehicle = new Vehicle(
                _db.NextVehicleId,
                RegistrationNumber,
                Make,
                (int)Odometer,
                (int)CostKm,
                Enum.Parse<VehicleTypes>(VehicleType),
                100,
                VehicleStatuses.Available
            );

            _db.Add(newVehicle as IVehicle);

            //Nollställning efter varje nytt försök
            Make = string.Empty;
            RegistrationNumber = string.Empty;
            Odometer = 0;
            CostKm = 0;
            VehicleType = string.Empty;
            Message = null;
        }
        catch (InputException ex)
        {
            Message = ex.Message;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred at:: {ex.Message}"); //Simulerar loggning så vi ser vart felet är ifall våran InputException inte skulle catcha felet.
            Message = "Error! Couldn't add a new vehicle";
        }
    }

    public void AddCustomer()
    {

        if (string.IsNullOrEmpty(LName) || string.IsNullOrEmpty(FName))
        {
            Message = "Error! Please enter Last Name and First Name.";
            return;
        }

        if (string.IsNullOrEmpty(SSN) || SSN.Length != 6 || !int.TryParse(SSN, out int ssn)) //TryParse för att konvertera SSN till en int. Detta för att input fältet ska kunna visa sin placeholder då en public int SSN skulle ha default värdet 0 redan från start. 
        {
            Message = "Error! SSN must have exactly 6 digits.";
            return;
        }

        if (_db.Get<IPerson>(p => p is Customer && (p as Customer).SocialSecurityNumber == ssn).Any()) //Any är en Linq metod som kollar om SSN redan existerar i Customer listan. Den returnerar en boolean beroende på om kriterierna möts eller ej.
        {
            Message = "Error! SSN already exists.";
            return;
        }

        try
        {
            var newCustomer = new Customer(
                _db.NextPersonId,
                ssn, 
                LName,
                FName
            );

            _db.Add(newCustomer as IPerson);

            //Nollställning 
            SSN = string.Empty;
            LName = string.Empty;
            FName = string.Empty;
            Message = null;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred at: {ex.Message}");
            Message = "Error! Couldn't add a new customer";
        }
    }

    //DEFAULT INTERFACE METHODS
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);

}

