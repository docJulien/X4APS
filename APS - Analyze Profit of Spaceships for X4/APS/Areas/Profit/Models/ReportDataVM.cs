using System;

namespace APS.Areas.Profit.Models
{
    public sealed class ReportDataVM
    {
        public string ActivityType { get; set; }
        public double Cycling { get; set; }
        public double Walking { get; set; }
        public double Running { get; set; }
        public double Hiking { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime MonthDate => new DateTime(Year,Month, 01);
    }
    public sealed class TransportDataVM
    {
        public string ActivityType { get; set; }
        public double InBus { get; set; }
        public double InPassengerVehicle { get; set; }
        public double InSubway { get; set; }
        public double InTrain { get; set; }
        public double Motorcycling { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime MonthDate => new DateTime(Year, Month, 01);
        public double Flying { get; set; }
    }
}