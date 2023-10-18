using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Exceptions;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Classes;
using Car_Rental.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Car_Rental.Business.Classes;

public class BookingProcessor
{
    private readonly IData _db;
    public Inputs Inputs { get; } = new Inputs();
    public BookingProcessor(IData db) => _db = db; //Pratar med våran datalager genom BookingProcessor och på så sätt få tillgång till det som finns i CollectionData, men endast efter att vi har injektserat det i Program.cs
   
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


    //BOOKINGS, VEHICLES, CUSTOMERS
    public async Task<IBooking> RentVehicle(int vehicleId, int customerId)
    {
        if (Inputs.SelectedCustomerId == 0) //Condition möts om ingen customer är vald och då är SelectedCustomerId = 0, vilket är default värdet.
        {
            Inputs.Message = "Error! Please pick a customer.";
            return null;
        }

        Inputs.Delay = true; //Boolean som används som condition när vi vill gråa ut våra knappar i html med disabled
        await Task.Delay(5000); // Simulerar att vi hämtar data från ett API med 5s fördröjning
        Inputs.Delay = false;

        return _db.RentVehicle(vehicleId, customerId);
    }

    public IBooking ReturnVehicle(int vehicleId, string distance)
    {
        if (!int.TryParse(distance, out int dist))
        {
            Inputs.Message = "Error! Please enter the distance.";
            return null;
        }

        var vehicle = _db.Get<IVehicle>(v => v.Id == vehicleId).FirstOrDefault();
        var booking = _db.Get<IBooking>(b => b.Vehicle.RegNo == vehicle.RegNo && b.EndRent == null).FirstOrDefault();

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
        Inputs.Distance = string.Empty;

        return booking;
    }

    public void AddVehicle()
    {

        try
        {
            if (string.IsNullOrEmpty(Inputs.VehicleInput.CarMake) || string.IsNullOrEmpty(Inputs.VehicleInput.RegNo) || string.IsNullOrEmpty(Inputs.VehicleInput.OdoMeter.ToString()) || string.IsNullOrEmpty(Inputs.VehicleInput.CostKm.ToString()))
            {
                throw new InputException("Error! Please fill all input fields with a value.");
            }

            if (_db.Get<IVehicle>(v => v is Vehicle && (v as Vehicle).RegNo == Inputs.VehicleInput.RegNo).Any()) //Any är en Linq metod som kollar om RegNo redan existerar i listan. Den returnerar en boolean beroende på om kriterierna möts eller ej.
            {                                                                                                    //Ingen specifik restriktion på RegNo för exempelvis längd då man kan ha custom registreringsnummer för fordon
                Inputs.Message = "Error! Registration Number already exists.";
                return;
            }

            var newVehicle = new Vehicle(
                _db.NextVehicleId,
                Inputs.VehicleInput.RegNo,
                Inputs.VehicleInput.CarMake,
                (int)Inputs.VehicleInput.OdoMeter,
                (int)Inputs.VehicleInput.CostKm,
                Inputs.VehicleInput.VType,
                100,
                VehicleStatuses.Available
            );

            _db.Add(newVehicle as IVehicle);

            //Nollställning efter varje nytt försök
            Inputs.VehicleInput.CarMake = string.Empty;
            Inputs.VehicleInput.RegNo = string.Empty;
            Inputs.VehicleInput.OdoMeter = 0;
            Inputs.VehicleInput.CostKm = 0;
            Inputs.Message = null;
        }
        catch (InputException ex)
        {
            Inputs.Message = ex.Message;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred at:: {ex.Message}"); //Simulerar loggning så vi ser vart felet är ifall våran InputException inte skulle catcha felet.
            Inputs.Message = "Error! Couldn't add a new vehicle";
        }
    }

    public void AddCustomer()
    {
        if (string.IsNullOrEmpty(Inputs.CustomerInput.LastName) || string.IsNullOrEmpty(Inputs.CustomerInput.FirstName))
        {
            Inputs.Message = "Error! Please enter Last Name and First Name.";
            return;
        }

        string ssn = Inputs.CustomerInput.SocialSecurityNumber;

        if (string.IsNullOrEmpty(ssn) || ssn.Length != 6)
        {
            Inputs.Message = "Error! SSN must have exactly 6 digits.";
            return;
        }

        if (_db.Get<IPerson>(p => p is Customer && (p as Customer).SocialSecurityNumber == ssn).Any()) // Any är en Linq metod som kollar om SSN redan existerar i Customer listan.Den returnerar en boolean beroende på om kriterierna möts eller ej.
        {
            Inputs.Message = "Error! SSN already exists.";
            return;
        }

        try
        {
            var newCustomer = new Customer(
                _db.NextPersonId,
                ssn,
                Inputs.CustomerInput.LastName,
                Inputs.CustomerInput.FirstName
            );

            _db.Add(newCustomer as IPerson);

            //Nollställning 
            Inputs.CustomerInput.SocialSecurityNumber = string.Empty;
            Inputs.CustomerInput.LastName = string.Empty;
            Inputs.CustomerInput.FirstName = string.Empty;
            Inputs.Message = null;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred at: {ex.Message}");
            Inputs.Message = "Error! Couldn't add a new customer";
        }
    }

    //DEFAULT INTERFACE METHODS
    public string[] VehicleStatusNames => _db.VehicleStatusNames;
    public string[] VehicleTypeNames => _db.VehicleTypeNames;
    public VehicleTypes GetVehicleType(string name) => _db.GetVehicleType(name);

}

