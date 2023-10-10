using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Interfaces;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Car_Rental.Business.Classes;


public class BookingProcessor
{
    private readonly IData _db;

    public BookingProcessor(IData db) => _db = db;

    public IEnumerable<IBooking> GetBookings()
    {
        return _db.GetBookings();
    }

    public IEnumerable<Customer> GetCustomers()
    {
        return _db.Get<IPerson>(p => p is Customer).OfType<Customer>();
    }

    public IEnumerable<IVehicle> GetVehicles(VehicleStatuses status = default)
    {
        return _db.Get<IVehicle>(v => status == default || v.VStatus == status);
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

    public IBooking RentVehicle(int vehicleId, int customerId)
    {
        return RentVehicleAsync(vehicleId, customerId).Result;
    }

    public async Task<IBooking> RentVehicleAsync(int vehicleId, int customerId)
    {
        await Task.Delay(2000); // Simulerar att vi hämtar data från ett API med 2s fördröjning

        return RentVehicle(vehicleId, customerId);
    }

    public IBooking ReturnVehicle(int vehicleId, double distance)
    {
        var vehicle = _db.GetVehicles().FirstOrDefault(v => v.RegNo == vehicleId.ToString());
        var booking = _db.GetBookings().FirstOrDefault(b => b.RegNo == vehicleId.ToString());

        if (vehicle == null || booking == null)
        {
            // Handle the case where the vehicle or booking is not found
            return null;
        }

        if (booking.KmReturned.HasValue)
        {
            // Handle the case where the booking has already been returned
            return null;
        }

        booking.KmReturned = distance;

        // Use the already obtained vehicle object to calculate the final cost
        double finalCost = (double)booking.GetCost(vehicle);

        // You may want to update other booking or vehicle details here

        return booking;
    }

    public void AddVehicle(string make, string registrationNumber, double odometer, double costKm, VehicleStatuses status, VehicleTypes type)
    {

        var newVehicle = new Vehicle( //skapar en instance av klassen Vehicle
            registrationNumber,
            make,
            (int)odometer,
            (int)costKm,
            type,
            0, // Value?
            status
        );

        _db.Vehicles.Add(newVehicle);
    }


    public void AddCustomer(int socialSecurityNumber, string firstName, string lastName)
    {
        var newCustomer = new Customer(
            socialSecurityNumber,
            firstName,
            lastName
        );

        _db.Persons.Add(newCustomer);

    }

    // Calling Default Interface Methods??
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);
}

/*
public class BookingProcessor
{
    private readonly IData _db;

    public BookingProcessor(IData db) => _db = db;



    public IEnumerable<Customer> GetCustomers()
     {
         var allPersons = _db.GetPersons();

         var customers = allPersons.OfType<Customer>(); //filtrerar och konverterat till customer objekt

         return customers;
     }
     public IEnumerable<IVehicle> GetVehicles(VehicleStatuses status = default) => _db.GetVehicles(status);
     public IEnumerable<IBooking> GetBookings() => _db.GetBookings();


    /*

    //alla getmetoder här ska anropa Get() från datalagret
    public IBooking GetBooking(int vehicleId) {}
    public IPerson? GetPerson(string ssn) { }
    public IVehicle? GetVehicle(int vehicleId) { }
    public IVehicle? GetVehicle(string regNo) { } 

    public async Task<IBooking> RentVehicle(int vehicleId, int customerId)
    {
        await Task.Delay(2000);

        IBooking bookingDelay = 

        return bookingDelay;

    }
    
    public IBooking ReturnVehicle(int vehicleId, double distance) { }
    public void AddVehicle(string make, string registrationNumber, double odometer, double costKm, VehicleStatuses status, VehicleTypes type)
    { }
    public void AddCustomer(string socialSecurityNumber, string firstName, string lastName)
    { }
    // Calling Default Interface Methods
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);

   

}*/