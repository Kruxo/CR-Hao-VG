using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Vehicle : IVehicle 
{
    public int Id { get; set; }
    public string RegNo { get; set; }
    public string CarMake { get; set; }
    public int OdoMeter { get; set; }
    public double CostKm { get; set; }
    public VehicleTypes VType { get; set; }
    public double CostDay { get; set; }
    public VehicleStatuses VStatus { get; set; }

    public Vehicle(int id, string regNo, string carMake, int odoMeter, int costKm, VehicleTypes vType, int costDay, VehicleStatuses vStatus)
    {
        Id = id;
        RegNo = regNo;
        CarMake = carMake;
        OdoMeter = odoMeter;
        CostKm = costKm;
        VType = vType;
        CostDay = costDay;
        VStatus = vStatus;
    }

}
