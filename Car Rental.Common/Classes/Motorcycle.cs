﻿using Car_Rental.Common.Enums;
using Car_Rental.Common.Interfaces;

namespace Car_Rental.Common.Classes;

public class Motorcycle : Vehicle
{
    public Motorcycle(int id, string regNo, string carMake, int odoMeter, int costKm, VehicleTypes vType, int costDay, VehicleStatuses vStatus) 
        : base(id, regNo, carMake, odoMeter, costKm, vType, costDay, vStatus)
    {
    }
}
