using System.IO;
using System;
using System.Runtime.CompilerServices;

namespace Car_Rental.Common.Extensions;

public static class VehicleExtensions
{
    public static double Duration(this DateTime startDate, DateTime endDate)
    {
        if (startDate.Date == endDate.Date)
        {
            return 1.0; // Ifall båda DateTime är samma dag så att det inte avrundas för nära noll eller lika med noll
                        // 1 timmes renting räknas ändå som 1 dag
        }

        return (endDate - startDate).TotalDays;
    }

}