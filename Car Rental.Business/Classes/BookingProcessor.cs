using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
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

    public int selectedCustomerId;
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
  
    /*public IBooking RentVehicle(int vehicleId, int customerId)
    {
        Task.Delay(2000).Wait(); // Simulate a delay

        return _db.RentVehicle(vehicleId, customerId);
    }*/

    public async Task<IBooking> RentVehicle(int vehicleId, int customerId)
    {
        await Task.Delay(2000); // Simulerar att vi hämtar data från ett API med 2s fördröjning

        return _db.RentVehicle(vehicleId, customerId);
    }


    public IBooking ReturnVehicle(int vehicleId, double distance)
    {
        var vehicle = _db.Get<IVehicle>(v => v.RegNo == vehicleId.ToString()).FirstOrDefault();
        var booking = _db.Get<IBooking>(b => b.RegNo == vehicleId.ToString()).FirstOrDefault();

        if (vehicle == null || booking == null)
        {
            return null; // Handle the case where the vehicle or booking is not found
        }

        if (booking.KmReturned.HasValue)
        {
            return null; // Handle the case where the booking has already been returned
        }

        booking.KmReturned = distance;

        double finalCost = (double)booking.GetCost(vehicle);  // Use the already obtained vehicle object to calculate the final cost

        return booking;
    }

    public void AddVehicle()
    {
        // Ensure VehicleType is set, defaulting to Convertible if not
        if (string.IsNullOrEmpty(VehicleType))
        {
            VehicleType = VehicleTypes.Convertible.ToString();
        }

        // Use the properties to add a new vehicle
        var newVehicle = new Vehicle(
            _db.NextVehicleId,
            RegistrationNumber,
            Make,
            (int)Odometer,
            (int)CostKm,
            Enum.Parse<VehicleTypes>(VehicleType),
            0, // Value?
            VehicleStatuses.Available
        );

        // Assuming _db has a method Add<T> to add items
        _db.Add(newVehicle as IVehicle);

        // Optionally, reset properties
        Make = string.Empty;
        RegistrationNumber = string.Empty;
        Odometer = 0;
        CostKm = 0;
        VehicleType = string.Empty;
    }

    public void AddCustomer()
    {
        var newCustomer = new Customer(
            _db.NextPersonId,
            SSN,
            LName,
            FName
        );

        _db.Add(newCustomer as IPerson);

        SSN = 0;
        LName = string.Empty;
        FName = string.Empty;
    }


    // Calling Default Interface Methods??
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);
}

