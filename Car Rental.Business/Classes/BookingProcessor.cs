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

    */

}