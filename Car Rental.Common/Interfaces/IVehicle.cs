using Car_Rental.Common.Enums;

namespace Car_Rental.Common.Interfaces;

public interface IVehicle
{
    public string RegNo { get; set; }
    public string CarMake { get; set; }
    public int OdoMeter { get; set; }
    public double CostKm { get; set; }
    public VehicleTypes VType { get; set; }
    public double CostDay { get; set; }
    public VehicleStatuses VStatus { get; set; }

}
