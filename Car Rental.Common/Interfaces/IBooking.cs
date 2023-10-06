using Car_Rental.Common.Enums;

namespace Car_Rental.Common.Interfaces;

public interface IBooking
{
    public string RegNo { get; set; }
    public string Customer { get; set; }
    public double KmRent { get; set; }
    public double? KmReturned { get; set; }
    public DateTime? StartRent { get; set; }
    public DateTime? EndRent { get; set; }
    public BookingStatuses Status { get; set; }
    public double? GetCost(IVehicle vehicle);


    //public IVehicle vehicle1 { get; set; }

}
