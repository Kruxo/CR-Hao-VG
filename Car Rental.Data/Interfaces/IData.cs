using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using System.Linq.Expressions;

namespace Car_Rental.Data.Interfaces;

public interface IData
{
    List<T> Get<T>(Expression<Func<T, bool>>? expression);
    T? Single<T>(Expression<Func<T, bool>>? expression);
    void Add<T>(T item);
    int NextVehicleId { get; }
    int NextPersonId { get; }
    int NextBookingId { get; }
    IBooking RentVehicle(int vehicleId, int customerId);
    IBooking ReturnVehicle(int vehicleId);

    // Default Interface Methods
    public string[] VehicleStatusNames => Enum.GetNames(typeof(VehicleStatuses));
    public string[] VehicleTypeNames => Enum.GetNames(typeof(VehicleTypes));

    public VehicleTypes GetVehicleType(string name);


    //vi behöver RentVehicle() och ReturnVehicle()
    //vi ska inte ha GetPersons, GetVehicles eller GetBookings. Vi ska ha en generisk metod som heter Get()
    //som ska ha lambda uttryck som man kan skicka in i parantesen som filtrerar data
    //som kan ex hämta alla med ett visst namn eller visst vehicle type. Denna returnerar en lista
    //Vi ska även ha en som heter Single som också är generisk som hämtar ett objekt typ en person, en bil eller en bokning
    //Sen en generisk metod som heter add


}