using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Exceptions;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Classes;
using Car_Rental.Data.Interfaces;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Car_Rental.Business.Classes;


public class BookingProcessor
{
    private readonly IData _db;
    public BookingProcessor(IData db) => _db = db;


    //RENT & RETURN VEHICLE
    public string Make { get; set; }
    public string RegistrationNumber { get; set; }
    public double Odometer { get; set; }
    public double CostKm { get; set; }
    public string VehicleType { get; set; }
    public string Distance { get; set; }


    //ADD CUSTOMER
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

    public IVehicle? GetVehicle(int vehicleId)
    {
        return _db.Single<IVehicle>(v => v.Id == vehicleId);
    }

    public IVehicle? GetVehicle(string regNo)
    {
        return _db.Single<IVehicle>(v => v.RegNo == regNo);
    }


    //BOOKING
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
        var booking = _db.Get<IBooking>(b => b.RegNo == vehicle.RegNo && b.EndRent == null).FirstOrDefault();

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
                VehicleType = VehicleTypes.Convertible.ToString(); //default selected vtype i våran dropdown om inget är vald, programmet krascha innan då den trodde att det var null
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
            Console.WriteLine($"Error occurred at:: {ex.Message}"); //simulerar loggning så vi ser vart felet är ifall våran InputException inte skulle catcha felet.
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

        try
        {
            var newCustomer = new Customer(
                _db.NextPersonId,
                ssn, // Use the parsed integer value
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

