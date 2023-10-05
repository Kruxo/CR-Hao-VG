using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Interfaces;

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

}