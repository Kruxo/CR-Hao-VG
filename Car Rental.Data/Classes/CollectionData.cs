using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Interfaces;
using System.Linq;

namespace Car_Rental.Data.Classes;

public class CollectionData : IData
{
    readonly List<IPerson> _persons = new List<IPerson>();
    readonly List<IVehicle> _vehicles = new List<IVehicle>();
    readonly List<IBooking> _bookings = new List<IBooking>();

    //public int NextVehicleId => _vehicles.Count.Equals(0) ? 1 : _vehicles.Max(b => b.Id) + 1; //skapar unikt Id då var använder oss av colletion o itne databaser
    //public int NextPersonId => _persons.Count.Equals(0) ? 1 : _persons.Max(b => b.Id) + 1;
    //public int NextBookingId => _bookings.Count.Equals(0) ? 1 : _bookings.Max(b => b.Id) + 1;

    public CollectionData() => SeedData();

    void SeedData()
    {
        _persons.Add(new Customer(123456, "Nguyen", "Hao"));
        _persons.Add(new Customer(654321, "Nguyen", "Pao"));

        _vehicles.Add(new Car("LOL777", "Saab", 50000, 2, VehicleTypes.Convertible, 200, (VehicleStatuses)2));
        _vehicles.Add(new Car("HA0420", "Volvo", 20000, 1, VehicleTypes.Bus, 300, (VehicleStatuses)2));
        _vehicles.Add(new Car("RIP666", "Wolkswagen", 10000, 2, VehicleTypes.Minivan, 500, (VehicleStatuses)1));
        _vehicles.Add(new Motorcycle("COW999", "Yamaha", 5000, 3, VehicleTypes.Motorcycle, 50, (VehicleStatuses)2));

        _bookings.Add(new Booking("RIP666", "Nguyen Hao (123456)", 1000.0, null, DateTime.Today.AddDays(-3), null, (BookingStatuses)1));
        _bookings.Add(new Booking("LOL777", "Nguyen Pao (654321)", 4000.0, 4000.0, DateTime.Today.AddDays(-6), DateTime.Today, (BookingStatuses)2));

    }
    public IEnumerable<IPerson> GetPersons() => _persons;
    public IEnumerable<IVehicle> GetVehicles(VehicleStatuses status = default) => _vehicles;
    public IEnumerable<IBooking> GetBookings() => _bookings;

    

}

