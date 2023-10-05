using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Booking : IBooking
{
    public string RegNo { get; set; }
    public string Customer { get; set; }
    public double KmRent { get; set; }
    public double? KmReturned { get; set; }
    public DateTime? StartRent { get; set; }
    public DateTime? EndRent { get; set; }
    public BookingStatuses Status { get; set; }

    public Booking(string regNo, string customer, double kmRent, double? kmReturned, DateTime? startRent, DateTime? endRent, BookingStatuses status)
    {
        RegNo = regNo;
        Customer = customer;
        KmRent = kmRent;
        KmReturned = kmReturned;
        StartRent = startRent;
        EndRent = endRent;
        Status = status;
    }

    public double? GetCost(IVehicle vehicle, IBooking booking)
    {
        if (booking.StartRent == null || booking.EndRent == null || booking.KmReturned == null)
        {
            return null;
        }

        DateTime startDate = booking.StartRent.Value;
        DateTime endDate = booking.EndRent.Value;

        double daysDifference = (endDate - startDate).TotalDays;
        return daysDifference * vehicle.CostDay + booking.KmReturned.Value * vehicle.CostKm;
    }

}



