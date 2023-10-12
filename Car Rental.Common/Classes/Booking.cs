using Car_Rental.Common.Enums;
using Car_Rental.Common.Extensions;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Booking : IBooking
{
    public int Id { get; set; }
    public string RegNo { get; set; }
    public string Customer { get; set; }
    public double KmRent { get; set; }
    public double? KmReturned { get; set; }
    public DateTime? StartRent { get; set; }
    public DateTime? EndRent { get; set; }
    public VehicleStatuses Status { get; set; }

    public Booking(int id, string regNo, string customer, double kmRent, double? kmReturned, DateTime? startRent, DateTime? endRent, VehicleStatuses status)
    {
        Id = id;
        RegNo = regNo;
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

        TimeSpan duration = this.EndRent.Value - this.StartRent.Value;
        double days = duration.TotalDays;

        return days * vehicle.CostDay + this.KmReturned.Value * vehicle.CostKm;
    }

}





