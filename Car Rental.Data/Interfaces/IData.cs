using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Data.Interfaces;

public interface IData
{
    public IEnumerable<IPerson> GetPersons();
    public IEnumerable<IVehicle> GetVehicles(VehicleStatuses status = default);
    public IEnumerable<IBooking> GetBookings();
}