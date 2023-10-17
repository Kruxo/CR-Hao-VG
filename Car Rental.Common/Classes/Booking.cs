using Car_Rental.Common.Enums;
using Car_Rental.Common.Extensions;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Booking : IBooking
{
    public int Id { get; set; }
    public IVehicle Vehicle { get; set; }
    public IPerson Customer { get; set; }
    public double KmRent { get; set; }
    public double? KmReturned { get; set; }
    public DateTime? StartRent { get; set; }
    public DateTime? EndRent { get; set; }
    public VehicleStatuses Status { get; set; }

    public Booking(int id, IVehicle vehicle, IPerson customer, double kmRent, double? kmReturned, DateTime? startRent, DateTime? endRent, VehicleStatuses status)
    {
        Id = id;
        Vehicle = vehicle;
        Customer = customer;
        KmRent = kmRent;
        KmReturned = kmReturned;
        StartRent = startRent;
        EndRent = endRent;
        Status = status;
    }

    public double? GetCost(IVehicle vehicle) 
    {
        if (this.StartRent == null || this.EndRent == null || this.KmReturned == null)
        {
            return null;
        }

        double days = this.StartRent.Value.Duration(this.EndRent.Value);
        return days * vehicle.CostDay + this.KmReturned.Value * vehicle.CostKm;
    }

}





