using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental.Common.InputsClasses;

public class DefaultVehicle : IVehicle
{
    public int Id { get; set; }
    public string RegNo { get; set; }
    public string CarMake { get; set; }
    public int OdoMeter { get; set; }
    public double CostKm { get; set; }
    public VehicleTypes VType { get; set; }
    public double CostDay { get; set; }
    public VehicleStatuses VStatus { get; set; }
}