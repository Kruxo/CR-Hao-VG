using System.Runtime.CompilerServices;

namespace Car_Rental.Common.Extensions;

public static class VehicleExtensions
{
    public static double Duration(this DateTime startDate, DateTime endDate) 
        => (startDate - endDate).TotalDays;

}