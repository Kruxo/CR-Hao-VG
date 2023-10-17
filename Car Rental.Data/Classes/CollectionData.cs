using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Car_Rental.Data.Classes;

public class CollectionData : IData
{
    readonly List<IPerson> _persons = new List<IPerson>();
    readonly List<IVehicle> _vehicles = new List<IVehicle>();
    readonly List<IBooking> _bookings = new List<IBooking>();

    public int NextVehicleId => _vehicles.Count.Equals(0) ? 1 : _vehicles.Max(b => b.Id) + 1; //Skapar unikt Id då vi använder oss av colletion o inte databaser så vi fejkar det
    public int NextPersonId => _persons.Count.Equals(0) ? 1 : _persons.Max(b => b.Id) + 1; //lägger till +1 på högsta ID:t om det finns, annars börjr vi på 1
    public int NextBookingId => _bookings.Count.Equals(0) ? 1 : _bookings.Max(b => b.Id) + 1;

    public CollectionData() => SeedData();

    void SeedData() //Sample data som finns redan vid start av webbapplikationen
    {
        IPerson person1 = new Customer(NextPersonId, 123456, "Nguyen", "Hao");
        IPerson person2 = new Customer(NextPersonId, 654321, "Nygren", "Hans");

        IVehicle vehicle1 = new Car(NextVehicleId, "LOL777", "Saab", 50000, 2, VehicleTypes.Convertible, 200, (VehicleStatuses)2);
        IVehicle vehicle2 = new Car(NextVehicleId, "HAO420", "Volvo", 20000, 1, VehicleTypes.Bus, 300, (VehicleStatuses)2);
        IVehicle vehicle3 = new Car(NextVehicleId, "RIP666", "Wolkswagen", 10000, 1, VehicleTypes.Minivan, 500, (VehicleStatuses)1);
        IVehicle vehicle4 = new Motorcycle(NextVehicleId, "COW999", "Yamaha", 5000, 3, VehicleTypes.Motorcycle, 50, (VehicleStatuses)2);

        _persons.AddRange(new IPerson[] { person1, person2 });
        _vehicles.AddRange(new IVehicle[] { vehicle1, vehicle2, vehicle3, vehicle4 });

        _bookings.Add(new Booking(NextBookingId, vehicle3, person1, 1000.0, null, DateTime.Today, null, (VehicleStatuses)2));
        _bookings.Add(new Booking(NextBookingId, vehicle1, person2, 4000.0, 4000.0, DateTime.Today.AddDays(-10), DateTime.Today, (VehicleStatuses)1));
    }

    public List<T> Get<T>(Expression<Func<T, bool>>? expression = null)
    {
        var collections = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<T>) && f.IsInitOnly)
            ?? throw new InvalidOperationException("Unsupported type");

        var value = collections.GetValue(this) ?? throw new InvalidDataException();

        var collection = ((List<T>)value).AsQueryable();

        return expression == null ? collection.ToList() : collection.Where(expression).ToList();
    }

    public T? Single<T>(Expression<Func<T, bool>>? expression = null)
    {
        var collections = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<T>) && f.IsInitOnly)
            ?? throw new InvalidOperationException("Unsupported type");

        var value = collections.GetValue(this) ?? throw new InvalidDataException();

        var collection = ((List<T>)value).AsQueryable();

        return expression == null ? collection.SingleOrDefault() : collection.SingleOrDefault(expression.Compile());
    }

    public void Add<T>(T item)
    {
        var collections = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<T>) && f.IsInitOnly)
            ?? throw new InvalidOperationException("Unsupported type");

        var value = collections.GetValue(this) ?? throw new InvalidDataException();

        ((List<T>)value).Add(item);
    }

    public IBooking? RentVehicle(int vehicleId, int customerId)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);

        if (vehicle == null || vehicle.VStatus != VehicleStatuses.Available)
        {
            return null;
        }

        var customer = _persons.OfType<Customer>().FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            return null;
        }

        vehicle.VStatus = VehicleStatuses.Booked;

        var booking = new Booking(
            NextBookingId,
            vehicle,
            customer,
            2000.0, 
            null,
            DateTime.Now,
            null,
            VehicleStatuses.Available
        );

        _bookings.Add(booking);

        return booking;
    }

    public IBooking? ReturnVehicle(int vehicleId)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);

        if (vehicle == null || vehicle.VStatus != VehicleStatuses.Booked)
        {
            return null;
        }

        var booking = _bookings.FirstOrDefault(b => b.VehicleBooking.RegNo == vehicle.RegNo && b.EndRent == null);

        if (booking == null)
        {
            return null;
        }

        return booking;
    }

    public string[] VehicleStatusNames => Enum.GetNames(typeof(VehicleStatuses));
    public string[] VehicleTypeNames => Enum.GetNames(typeof(VehicleTypes));
    public VehicleTypes GetVehicleType(string name) => Enum.TryParse<VehicleTypes>(name, out var result) ? result : default;  //Konverterar och returnerar en string enum till ett enum värde

}

