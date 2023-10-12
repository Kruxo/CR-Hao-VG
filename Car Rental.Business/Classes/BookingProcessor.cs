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

 
    public string Make { get; set; }
    public string RegistrationNumber { get; set; }
    public double Odometer { get; set; }
    public double CostKm { get; set; }
    public string VehicleType { get; set; }

    public int SSN { get; set; } 
    public string LName { get; set; }
    public string FName { get; set; }

    public bool Delay { get; set; }
    public bool Processing { get; set; }

    public int SelectedCustomerId { get; set; }
    public int Distance {  get; set; }
   
    public string Message { get; private set; }

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

    public List<IVehicle> GetAllVehicles()
    {
        return _db.Get<IVehicle>(null);
    }

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

    public async Task<IBooking> RentVehicle(int vehicleId, int customerId)
    {
        Delay = true; //Boolean som används som condition när vi vill gråa ut våra knappar i html med disabled
        await Task.Delay(5000); // Simulerar att vi hämtar data från ett API med 5s fördröjning
        Delay = false;

        return _db.RentVehicle(vehicleId, customerId);
    }

    public IBooking ReturnVehicle(int vehicleId, double distance)
    {
        var vehicle = _db.Get<IVehicle>(v => v.RegNo == vehicleId.ToString()).FirstOrDefault();
        var booking = _db.Get<IBooking>(b => b.RegNo == vehicleId.ToString()).FirstOrDefault();

        if (vehicle == null || booking == null)
        {
            return null; //Ifall vi inte hittar vårat fordon eller booking
        }

        if (booking.KmReturned.HasValue)
        {
            return null; //ifall vi redan returnerat bookningen
        }

        booking.KmReturned = distance;

        double finalCost = (double)booking.GetCost(vehicle);  

        return booking;
    }
    
    public void AddVehicle()
    {
        try
        {
            if (string.IsNullOrEmpty(Make) || string.IsNullOrEmpty(RegistrationNumber) || string.IsNullOrEmpty(Odometer.ToString()) || string.IsNullOrEmpty(CostKm.ToString()))
            {
                throw new InputException("Please fill all input fields with a value.");
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
                3000,
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
            Console.WriteLine($"An error occurred: {ex.Message}"); //simulerar loggning så vi ser vart felet är ifall våran InputException inte skulle catcha felet.
            Message = "An error occurred while adding a new vehicle.";
        }
    }



    public void AddCustomer()
    {
        string ssnCondition = SSN.ToString();

        if (string.IsNullOrWhiteSpace(LName) || string.IsNullOrWhiteSpace(FName))
        {
            Message = "Please enter Last Name and First Name.";
            return;
        }

        if (ssnCondition.Length != 6)
        {
            Message = "SSN must have exactly 6 digits.";
            return;
        }

        try
        {
            var newCustomer = new Customer(
                _db.NextPersonId,
                SSN,
                LName,
                FName
            );

            _db.Add(newCustomer as IPerson);

            //Nollställning efter varje nytt försök

            LName = string.Empty;
            FName = string.Empty;
            Message = null; 
        }

        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}"); 
            Message = "An error occurred while adding a new customer.";
        }
    }


    //Default Interface Methods
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);
}

