using System;

namespace BusinessLogic.Models
{
    public class Ship : IEquatable<Ship>
    {
        public Ship()
        {
        }
        public string ShipID { get; set; }
        public string ShipName { get; set; }

        public bool Actif { get; set; }

         public bool Equals(Ship other)
        {
            return ShipID == other.ShipID;
        }

        public override int GetHashCode()
        {
            return ShipID.GetHashCode();
        }
    }
}
