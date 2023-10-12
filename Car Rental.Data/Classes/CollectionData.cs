using Car_Rental.Common.Classes;
using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using Car_Rental.Data.Interfaces;
using System.Linq;
using System.Linq.Expressions;

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
        _persons.Add(new Customer(NextPersonId, 123456, "Nguyen", "Hao"));
        _persons.Add(new Customer(NextPersonId, 654321, "Alving", "Paulina"));

        _vehicles.Add(new Car(NextVehicleId, "LOL777", "Saab", 50000, 2, VehicleTypes.Convertible, 200, (VehicleStatuses)2));
        _vehicles.Add(new Car(NextVehicleId, "HAO420", "Volvo", 20000, 1, VehicleTypes.Bus, 300, (VehicleStatuses)2));
        _vehicles.Add(new Car(NextVehicleId, "RIP666", "Wolkswagen", 10000, 1, VehicleTypes.Minivan, 500, (VehicleStatuses)1));
        _vehicles.Add(new Motorcycle(NextVehicleId, "COW999", "Yamaha", 5000, 3, VehicleTypes.Motorcycle, 50, (VehicleStatuses)2));

        _bookings.Add(new Booking(NextBookingId, "RIP666", "Nguyen Hao (123456)", 1000.0, null, DateTime.Today, null, (VehicleStatuses)2));
        _bookings.Add(new Booking(NextBookingId, "LOL777", "Alving Paulina (654321)", 4000.0, 4000.0, DateTime.Today.AddDays(-10), DateTime.Today, (VehicleStatuses)1));
    }

    public List<T> Get<T>(Expression<Func<T, bool>>? expression)
    {
        if (expression == null)
        {
            // Returnera hela listan om expression är null
            if (typeof(T) == typeof(IPerson))
            {
                return _persons.OfType<T>().ToList();
            }
            else if (typeof(T) == typeof(IVehicle))
            {
                return _vehicles.OfType<T>().ToList();
            }
            else if (typeof(T) == typeof(IBooking))
            {
                return _bookings.OfType<T>().ToList();
            }
            else
            {
                return new List<T>();
            }
        }
        else
        {
            // Om expression inte är null kan vi filtrera med Linq
            if (typeof(T) == typeof(IPerson))
            {
                return _persons.OfType<T>().Where(expression.Compile()).ToList();
            }
            else if (typeof(T) == typeof(IVehicle))
            {
                return _vehicles.OfType<T>().Where(expression.Compile()).ToList();
            }
            else if (typeof(T) == typeof(IBooking))
            {
                return _bookings.OfType<T>().Where(expression.Compile()).ToList();
            }
            else
            {
                return new List<T>();
            }
        }
    }

    public T? Single<T>(Expression<Func<T, bool>>? expression)
    {
        if (expression == null)
        {
            return default;
        }

        // Typ av T kollas upp och letar efter ett enda item baserat på våran expression 
        if (typeof(T) == typeof(IPerson))
        {
            return _persons.OfType<T>().SingleOrDefault(expression.Compile());
        }
        else if (typeof(T) == typeof(IVehicle))
        {
            return _vehicles.OfType<T>().SingleOrDefault(expression.Compile());
        }
        else if (typeof(T) == typeof(IBooking))
        {
            return _bookings.OfType<T>().SingleOrDefault(expression.Compile());
        }
        else
        {
            return default;
        }
    }

    public void Add<T>(T item)
    {
        if (typeof(T) == typeof(IPerson))
        {
            _persons.Add(item as IPerson);
        }
        else if (typeof(T) == typeof(IVehicle))
        {
            _vehicles.Add(item as IVehicle);
        }
        else if (typeof(T) == typeof(IBooking))
        {
            _bookings.Add(item as IBooking);
        }
        else
        {
            
        }
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
            NextVehicleId,
            vehicle.RegNo,
            $"{customer.LastName} {customer.FirstName} ({customer.SocialSecurityNumber})",
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

        var booking = _bookings.FirstOrDefault(b => b.RegNo == vehicle.RegNo && b.EndRent == null);

        if (booking == null)
        {
            return null;
        }

        return booking;
    }

    public string[] VehicleStatusNames => Enum.GetNames(typeof(VehicleStatuses));
    public string[] VehicleTypeNames => Enum.GetNames(typeof(VehicleTypes));
    public VehicleTypes GetVehicleType(string name) //Konverterar ett string enum till ett enum värde 
    {
        if (Enum.TryParse(typeof(VehicleTypes), name, out object result))
        {
            return (VehicleTypes)result;
        }

        return default(VehicleTypes);
    }
}

